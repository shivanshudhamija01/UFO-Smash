using UnityEngine;

public class AnimalAbduct
    : BaseState<AnimalController>
{
    private Transform transform;
    private Transform visual;

    private Transform abductTarget;

    private bool isMovingLeftToRight;

    private float tiltAngle;
    private float tiltSpeed;
    private float abductingSpeed;

    private float targetTilt;
    private float captureDistance = 0.5f;
    private UFOController uFOController;

    public AnimalAbduct(AnimalController controller) : base(controller)
    {
    }

    public override void OnEnterState()
    {
        Init();
        targetTilt = tiltAngle;
        Debug.Log("The value of moving isLeftToRight is : " + isMovingLeftToRight);

        Debug.Log("Animal entered into abduct state");
    }

    public override void UpdateState()
    {
        if (abductTarget == null) return;

        MoveTowardsUFO();
        TiltVisual();
        CheckCapture();
    }

    public override void OnExitState()
    {
        if (visual != null)
        {
            visual.localRotation = Quaternion.identity;
        }
    }

    public override void FixedUpdateState()
    {
    }

    private void Init()
    {
        transform = controller.GetTransform();

        visual = controller.GetVisualTransform();

        abductTarget = controller.GetAbductTarget();

        isMovingLeftToRight = controller.IsMovingLeftToRight();

        tiltAngle = controller.GetTiltAngle();

        tiltSpeed = controller.GetTiltSpeed();

        abductingSpeed = controller.GetAbductingSpeed();
        uFOController = controller.GetCurrentUFO();
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