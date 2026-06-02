using UnityEngine;

public class AnimalAbduct
    : BaseState<AnimalController>
{
    private Transform transform;
    private Transform visual;
    private Transform abductTarget;

    private float tiltAngle;
    private float tiltSpeed;
    private float abductingSpeed;

    private float targetTilt;
    private float captureDistance = 0.5f;
    private UFOController uFOController;
    private Animator animator;
    private int key = Animator.StringToHash("IsCaught");
    public AnimalAbduct(AnimalController controller) : base(controller)
    {
    }

    public override void OnEnterState()
    {
        Debug.Log("Entered into the caught state");
        Init();
        targetTilt = tiltAngle;
    }

    public override void UpdateState()
    {
        if (abductTarget == null)
        {
            controller.ReleaseFromAbduction();
            return;
        }
        MoveTowardsUFO();
        TiltVisual();
        CheckCapture();
    }

    public override void OnExitState()
    {
        // if (visual != null)
        // {
        //     visual.localRotation = Quaternion.identity;
        // }
    }

    public override void FixedUpdateState()
    {
    }

    private void Init()
    {
        transform = controller.GetTransform();

        visual = controller.GetVisualTransform();

        abductTarget = controller.GetAbductTarget();

        tiltAngle = controller.GetTiltAngle();

        tiltSpeed = controller.GetTiltSpeed();

        abductingSpeed = controller.GetAbductingSpeed();
        uFOController = controller.GetCurrentUFO();
        if (animator == null)
        {
            animator = controller.GetAnimator();
        }
        animator.SetTrigger(key);
    }

    private void MoveTowardsUFO()
    {
        transform.position = Vector2.MoveTowards(transform.position, abductTarget.position, abductingSpeed * Time.deltaTime);
    }

    private void TiltVisual()
    {
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetTilt);

        visual.localRotation = Quaternion.Lerp(visual.localRotation, targetRotation, tiltSpeed * Time.deltaTime);
    }
    private void CheckCapture()
    {
        float distance = Vector2.Distance(transform.position, abductTarget.position);

        if (distance <= captureDistance)
        {
            // Change UFO state
            if (uFOController != null)
            {
                uFOController.GetStateMachine().ChangeState(UFOStates.success);
            }

            // Change Animal state
            controller.GetStateMachine().ChangeState(AnimalState.taken);
        }
    }
}


// May be here i have to add the animalService here so that 