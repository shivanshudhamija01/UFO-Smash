using UnityEngine;
using UnityEngine.Splines;
public class UFOSpline : BaseState<UFOController>
{
    private Transform transform;
    private SplineContainer spline;
    private float splineDistance;
    private float splineLength;
    private float moveSpeed;
    private bool rotateAlongSpline;
    private Animator animator;
    private IAudioService audioService;
    private UFOType ufoType;
    private readonly int key = Animator.StringToHash("IsSpline");
    public UFOSpline(UFOController controller)
        : base(controller)
    {
    }

    public override void OnEnterState()
    {
        transform = controller.GetTransform();

        spline = controller.GetSpline();

        moveSpeed = controller.GetSplineSpeed();

        rotateAlongSpline = controller.ShouldRotateSpline();

        splineDistance = 0;
        splineLength = spline.CalculateLength();
        ufoType = controller.GetUFOType(); 
        if (audioService == null)
        {
            audioService = ServiceLocator.Get<IAudioService>();
        }
        if (ufoType == UFOType.Boss)
        {
            audioService.SFX(SoundType.BossUFO);
        }
        else
        {
            audioService.SFX(SoundType.UFOEntry);
        }
    }

    public override void UpdateState()
    {
        splineDistance += moveSpeed * Time.deltaTime;

        float normalizedDistance = splineDistance / splineLength;

        normalizedDistance = Mathf.Clamp01(normalizedDistance);

        Vector3 position = spline.EvaluatePosition(normalizedDistance);

        transform.position = position;

        if (rotateAlongSpline)
        {
            Vector3 tangent = spline.EvaluateTangent(normalizedDistance);

            float angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, 0, angle + 180f);
        }

        if (normalizedDistance >= 0.75f)
        {
            controller.GetStateMachine().ChangeState(UFOStates.hover);
        }
    }

    public override void OnExitState()
    {
        // animator.SetBool(key, false);
    }

    public override void FixedUpdateState()
    {
    }
}

