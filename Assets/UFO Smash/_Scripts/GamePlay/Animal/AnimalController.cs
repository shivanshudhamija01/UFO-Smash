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
    private void OnEnable()
    {
        animalStateMachine.ChangeState(AnimalState.sleep);
    }
    private void OnDisable()
    {
        animalStateMachine.ChangeState(AnimalState.sleep);
    }
    public void Initialize(Transform target, AnimalSpawner animalSpawner, Lane assignedLane, int layer)
    {
        targetPoint = target;
        spawner = animalSpawner;
        lane = assignedLane;
        spriteRenderer.sortingOrder = layer;
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
    public Transform GetTargetPoint() => targetPoint;
    public AnimalSpawner GetAnimalSpawner() => spawner;
    public Lane GetAssignedLane() => lane;
    public float GetMovingSpeed() => moveSpeed;
    public Transform GetTransform() => transform;

}
