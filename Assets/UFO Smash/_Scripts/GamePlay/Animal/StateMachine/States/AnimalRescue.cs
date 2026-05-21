using UnityEngine;

public class AnimalRescue : BaseState<AnimalController>
{
    private Transform animalTransform;
    private Transform visual;

    private float moveSpeed;
    private float tiltSpeed;

    private float targetY;

    private bool reachedGround;

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

        moveSpeed = controller.GetAbductingSpeed();

        tiltSpeed = controller.GetTiltSpeed();

        targetY = controller.TargetPoint.y;

        reachedGround = false;
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
        // Debug.Log("I am inside the rescue state and current Pos and traget pos is : " + currentPos + " " + targetPos);
        // Landed
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

            controller.GetStateMachine().ChangeState(AnimalState.roam);
        }
    }
}