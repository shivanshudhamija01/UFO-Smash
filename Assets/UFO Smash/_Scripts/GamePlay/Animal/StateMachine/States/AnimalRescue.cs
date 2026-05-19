using UnityEngine;

public class AnimalRescue
    : BaseState<AnimalController>
{
    private Transform transform;
    private Transform visual;

    private Transform targetPoint;

    private float moveSpeed;
    private float tiltSpeed;

    private float targetY;

    private bool reachedGround;

    private const float reachThreshold = 0.1f;

    private const float rotationThreshold = 1f;

    public AnimalRescue(AnimalController controller) : base(controller)
    {
    }

    public override void OnEnterState()
    {
        transform = controller.GetTransform();

        visual = controller.GetVisualTransform();

        targetPoint = controller.GetTargetPoint();

        moveSpeed = controller.GetAbductingSpeed();

        tiltSpeed = controller.GetTiltSpeed();

        targetY = targetPoint.position.y;

        reachedGround = false;

        Debug.Log("Animal is rescued");
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
        visual.localRotation =
            Quaternion.identity;
    }

    public override void FixedUpdateState()
    {
    }

    private void MoveToGround()
    {

        Vector3 currentPos = transform.position;

        Vector3 targetPos = new Vector3(currentPos.x, targetY, currentPos.z);

        transform.position = Vector3.MoveTowards(currentPos, targetPos, moveSpeed * Time.deltaTime);

        // Reached ground
        if (Mathf.Abs(transform.position.y - targetY) <= reachThreshold)
        {
            transform.position = targetPos;

            reachedGround = true;
        }
    }

    private void TiltBack()
    {
        Debug.Log("Tilt Back is called");
        visual.localRotation = Quaternion.Lerp(visual.localRotation, Quaternion.identity, tiltSpeed * Time.deltaTime);

        float currentZ = Mathf.Abs(visual.localEulerAngles.z);

        // Rotation recovered
        if (Quaternion.Angle(visual.localRotation, Quaternion.identity) <= rotationThreshold)
        {
            visual.localRotation = Quaternion.identity;

            controller.GetStateMachine().ChangeState(AnimalState.roam);
        }
    }
}