using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Splines;

public class UFOController : MonoBehaviour
{
    [Header("Spline")]
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private float splineMoveSpeed = 5f;
    [SerializeField] private bool rotateAlongSpline = true;

    [Header("Movement")]
    [SerializeField] private float manualMoveSpeed = 3f;
    [SerializeField] private Vector2 offset;
    [SerializeField] private float tiltSpeed;

    [Header("References")]
    [SerializeField] private Light2D torchLight;
    [SerializeField] private Transform lockedAnimal;

    private UFOStateMachine stateMachine;

    private void Awake()
    {
        stateMachine =
            new UFOStateMachine(this);
    }
    // Remove this start method later and call the initialize method in spawning
    private void Start()
    {
        Initialize(lockedAnimal);
    }
    public void Initialize(Transform targetAnimal)
    {
        lockedAnimal = targetAnimal;

        stateMachine.ChangeState(
            UFOStates.spline
        );
    }

    private void Update()
    {
        stateMachine.Update();
    }

    // Getters
    public Transform GetTransform() => transform;
    public Transform GetLockedAnimal() => lockedAnimal;
    public Light2D GetTorchLight() => torchLight;

    public SplineContainer GetSpline() => splineContainer;
    public float GetSplineSpeed() => splineMoveSpeed;
    public bool ShouldRotateSpline() => rotateAlongSpline;

    public float GetManualSpeed() => manualMoveSpeed;
    public Vector2 GetOffset() => offset;

    public UFOStateMachine GetStateMachine()
        => stateMachine;
}

// I got it why the UFO movement is not working , because the initialize is called in Spawning script of both the animal and UFO controller
