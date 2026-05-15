using UnityEngine;

public class AnimalRoam : BaseState<AnimalController>
{
    private Transform targetPoint;
    private AnimalSpawner animalSpawner;
    private Lane lane;
    private float moveSpeed;
    private Transform transform;
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
            targetPoint.position,
            moveSpeed * Time.deltaTime);

        Vector3 dir = targetPoint.position - transform.position;

        // Flip
        if (dir.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);

        // Reached destination
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            ReturnToPool();
        }
    }
    public override void OnExitState()
    {
        Debug.Log("Animal Roam Exit");
    }
    public override void FixedUpdateState()
    {

    }
    private void Init()
    {
        targetPoint = controller.GetTargetPoint();
        animalSpawner = controller.GetAnimalSpawner();
        lane = controller.GetAssignedLane();
        moveSpeed = controller.GetMovingSpeed();
        transform = controller.GetTransform();
    }
    private void ReturnToPool()
    {
        lane.currentAnimals--;

        animalSpawner.AnimalRemoved(this.controller);

        transform.gameObject.SetActive(false);
    }
}
