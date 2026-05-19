using UnityEngine;

public class AnimalController : MonoBehaviour, IAbductable
{
    // i think i have to set the reference of the speed here, and then 
    // set accordingly to animal differently
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform visualTransform;
    [SerializeField] private float tiltSpeed = 5f;
    [SerializeField] private float abductingSpeed = 10f;
    [SerializeField] private float tiltAngle = 40f;
    private Transform targetPoint;
    private AnimalSpawner spawner;
    private Lane lane;
    private bool isMoving;
    private SpriteRenderer spriteRenderer;
    private AnimalStateMachine animalStateMachine;
    #region  Abduct
    private Transform abductTarget;
    private bool leftToRight;
    private UFOController currentUFO;
    #endregion
    private void Awake()
    {
        animalStateMachine = new AnimalStateMachine(this);
        spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
    }
    private void OnEnable()
    {
        animalStateMachine.ChangeState(AnimalState.sleep);
    }
    private void OnDisable()
    {
        animalStateMachine.ChangeState(AnimalState.sleep);
    }
    public void Initialize(Transform target, AnimalSpawner animalSpawner, Lane assignedLane, int layer, bool lTor)
    {
        targetPoint = target;
        spawner = animalSpawner;
        lane = assignedLane;
        spriteRenderer.sortingOrder = layer;
        leftToRight = lTor;
        animalStateMachine.ChangeState(AnimalState.roam);
        // May be here i need to add the logic for isMoving, but as it is a state machine perhaps , it works without it
        // isMoving = true;
    }
    void Update()
    {
        if (animalStateMachine != null)
        {
            animalStateMachine.Update();
        }
    }
    public void BeginAbduction(Transform abductTarget, UFOController ufo)
    {
        this.abductTarget = abductTarget;
        this.currentUFO = ufo;
        animalStateMachine.ChangeState(AnimalState.abducting);
    }

    public void CancelAbduction()
    {
        animalStateMachine.ChangeState(AnimalState.rescue);
    }
    public Transform GetTargetPoint() => targetPoint;
    public AnimalSpawner GetAnimalSpawner() => spawner;
    public Lane GetAssignedLane() => lane;
    public float GetMovingSpeed() => moveSpeed;
    public Transform GetTransform() => transform;
    public Transform GetAbductTarget() => abductTarget;
    public bool IsMovingLeftToRight() => leftToRight;
    public float GetTiltAngle() => tiltAngle;
    public float GetTiltSpeed() => tiltSpeed;
    public float GetAbductingSpeed() => abductingSpeed;
    public Transform GetVisualTransform() => visualTransform;
    public UFOController GetCurrentUFO() => currentUFO;
    public AnimalStateMachine GetStateMachine() => animalStateMachine;
}
