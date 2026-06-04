using UnityEngine;

public class AnimalRoam : BaseState<AnimalController>
{
    private Vector3 targetPoint;
    private AnimalSpawner animalSpawner;
    private Lane lane;
    private float moveSpeed;
    private Transform transform;
    private Animator animator;
    private int key = Animator.StringToHash("IsWalking");
    public AnimalRoam(AnimalController controller) : base(controller)
    {
    }
    public override void OnEnterState()
    {
        Init();
    }
    public override void UpdateState()
    {
        if (targetPoint == null)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPoint,
            moveSpeed * Time.deltaTime);


        // Reached destination
        if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
        {
            if (!controller.IsLocked())
                ReturnToPool();
        }
    }
    public override void OnExitState()
    {
    }
    public override void FixedUpdateState()
    {

    }
    private void Init()
    {
        targetPoint = controller.TargetPoint;
        animalSpawner = controller.GetAnimalSpawner();
        lane = controller.AssignedLane;
        moveSpeed = controller.MoveSpeed;
        transform = controller.GetTransform();
        if (animator == null)
        {
            animator = controller.GetAnimator();
        }
        animator.SetTrigger(key);
    }
    private void ReturnToPool()
    {
        // lane.currentAnimals = Mathf.Max(0, lane.currentAnimals - 1);

        animalSpawner.AnimalRemoved(this.controller);

        transform.gameObject.SetActive(false);
    }
}
