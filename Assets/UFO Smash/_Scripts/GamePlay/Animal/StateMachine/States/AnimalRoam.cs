using UnityEngine;

public class AnimalRoam : BaseState<AnimalController>
{
    private Vector3 targetPoint;
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
        Debug.Log("Move speed of animal in roam state is : " + moveSpeed + " " + transform.gameObject.name);
        Debug.Log("Animal position on roam enter: " + transform.position);
        Debug.Log("Target position on roam enter: " + targetPoint);
        Debug.Log("Distance on roam enter: " + Vector3.Distance(transform.position, targetPoint));
    }
    public override void UpdateState()
    {
        if (targetPoint == null)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPoint,
            moveSpeed * Time.deltaTime);

        // Vector3 dir = targetPoint.position - transform.position;

        // // Flip
        // if (dir.x > 0)
        //     transform.localScale = new Vector3(1, 1, 1);
        // else
        //     transform.localScale = new Vector3(-1, 1, 1);
        // Reached destination
        if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
        {
            ReturnToPool();
        }
    }
    public override void OnExitState()
    {
        // Debug.Log("Animal Roam Exit");
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
        Debug.Log("Entered into animal roam state : " + transform.gameObject.name);
        // Debug.Log("Transform position is : " + transform.position);
        // Debug.Log("Target Position is : " + targetPoint.position);
    }
    private void ReturnToPool()
    {
        lane.currentAnimals = Mathf.Max(0, lane.currentAnimals - 1);

        animalSpawner.AnimalRemoved(this.controller);

        transform.gameObject.SetActive(false);
    }
}
