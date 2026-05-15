using UnityEngine;

public class AnimalController : MonoBehaviour
{
    // i think i have to set the reference of the speed here, and then 
    // set accordingly to animal differently
    [SerializeField] private float moveSpeed;
    private Transform targetPoint;
    private AnimalSpawner spawner;
    private Lane lane;
    private bool isMoving;
    private SpriteRenderer spriteRenderer;
    private AnimalStateMachine animalStateMachine;
    private void Awake()
    {
        animalStateMachine = new AnimalStateMachine(this);
        spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
    }
    public void Initialize(Transform target, AnimalSpawner animalSpawner, Lane assignedLane, int layer)
    {
        targetPoint = target;
        spawner = animalSpawner;
        lane = assignedLane;
        spriteRenderer.sortingOrder = layer;
        // May be here i need to add the logic for isMoving, but as it is a state machine perhaps , it works without it
        // isMoving = true;
    }
    private void Start()
    {
        animalStateMachine.ChangeState(AnimalState.roam);
    }
    void Update()
    {
        if (animalStateMachine != null)
        {
            animalStateMachine.Update();
        }
    }
    public Transform GetTargetPoint() => targetPoint;
    public AnimalSpawner GetAnimalSpawner() => spawner;
    public Lane GetAssignedLane() => lane;
    public float GetMovingSpeed() => moveSpeed;
    public Transform GetTransform() => transform;

}
