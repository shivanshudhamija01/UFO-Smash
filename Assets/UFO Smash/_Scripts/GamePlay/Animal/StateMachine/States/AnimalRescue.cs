using UnityEngine;

public class AnimalRescue : BaseState<AnimalController>
{
    private Transform animalTransform;
    private Transform visual;
    private IAnimalService animalService;
    private float moveSpeed;
    private float tiltSpeed;

    private float targetY;
    private float gravity = 5f;

    private bool reachedGround;
    private float tiltSpeedModifier = 4f;
    private const float reachThreshold = 0.1f;
    private const float rotationThreshold = 2f;

    public AnimalRescue(AnimalController controller)
        : base(controller)
    {
    }

    public override void OnEnterState()
    {
        animalTransform = controller.GetTransform();

        visual = controller.GetVisualTransform();

        moveSpeed = controller.GetAbductingSpeed() * gravity;

        tiltSpeed = controller.GetTiltSpeed() * tiltSpeedModifier;

        targetY = controller.TargetPoint.y;

        reachedGround = false;
        if (animalService == null)
        {
            animalService = ServiceLocator.Get<IAnimalService>();
        }
    }

    public override void UpdateState()
    {
        if (!reachedGround)
        {
            MoveToGround();
        }
        else
        {
            TiltBack();
        }
    }

    public override void OnExitState()
    {
        visual.localRotation = Quaternion.identity;
    }

    public override void FixedUpdateState()
    {
    }

    private void MoveToGround()
    {
        Vector3 currentPos = animalTransform.position;

        Vector3 targetPos = new Vector3(currentPos.x, targetY, currentPos.z);

        animalTransform.position = Vector2.MoveTowards(currentPos, targetPos, moveSpeed * Time.deltaTime);

        if (Mathf.Abs(animalTransform.position.y - targetY) <= reachThreshold)
        {
            animalTransform.position = targetPos;
            reachedGround = true;
        }
    }

    private void TiltBack()
    {
        visual.localRotation = Quaternion.Lerp(visual.localRotation, Quaternion.identity, tiltSpeed * Time.deltaTime);

        if (Quaternion.Angle(visual.localRotation, Quaternion.identity) <= rotationThreshold)
        {
            visual.localRotation = Quaternion.identity;
            animalService.AddAnimal(this.controller);
            controller.GetStateMachine().ChangeState(AnimalState.roam);

        }
    }
}