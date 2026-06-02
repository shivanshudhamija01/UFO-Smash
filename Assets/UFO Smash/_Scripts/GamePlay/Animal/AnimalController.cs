using Unity.VisualScripting;
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
    [SerializeField] private Animator animator;
    [SerializeField] private AnimalType animalType;
    private Vector3 targetPoint;
    private AnimalSpawner spawner;
    private Lane lane;
    private bool isMoving;
    private SpriteRenderer spriteRenderer;
    private AnimalStateMachine animalStateMachine;
    #region  Abduct
    private Transform abductTarget;
    private bool leftToRight;
    private UFOController currentUFO;
    private bool isLocked;
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
        targetPoint = target.position;
        targetPoint.z = 0;
        spawner = animalSpawner;
        lane = assignedLane;
        spriteRenderer.sortingOrder = layer;
        leftToRight = lTor;
        animalStateMachine.ChangeState(AnimalState.roam);
        visualTransform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        transform.localScale = leftToRight ? Vector3.one : new Vector3(-1, 1, 1);
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
        // Here i am little confused to set it true or false
        isLocked = true;
        this.abductTarget = abductTarget;
        this.currentUFO = ufo;
        animalStateMachine.ChangeState(AnimalState.abducting);
    }

    public void ReleaseFromAbduction()
    {
        isLocked = false;
        currentUFO = null;
        animalStateMachine.ChangeState(AnimalState.rescue);
    }
    public Vector3 TargetPoint => targetPoint;
    public Lane AssignedLane => lane;
    public float MoveSpeed => moveSpeed;
    public AnimalSpawner GetAnimalSpawner() => spawner;
    public Transform GetTransform() => transform;
    public Transform GetAbductTarget() => abductTarget;
    public bool IsMovingLeftToRight() => leftToRight;
    public float GetTiltAngle() => tiltAngle;
    public float GetTiltSpeed() => tiltSpeed;
    public float GetAbductingSpeed() => abductingSpeed;
    public Transform GetVisualTransform() => visualTransform;
    public UFOController GetCurrentUFO() => currentUFO;
    public AnimalStateMachine GetStateMachine() => animalStateMachine;
    public int GetSortingOrder() => spriteRenderer.sortingOrder;
    public bool IsLocked() => isLocked;
    public Animator GetAnimator() => animator;
    public AnimalType GetAnimalType() => animalType;
    public void SetLocked(bool locked)
    {
        isLocked = locked;
    }
}
