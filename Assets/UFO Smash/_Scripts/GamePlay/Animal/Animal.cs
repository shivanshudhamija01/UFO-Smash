using UnityEngine;

public class Animal : MonoBehaviour
{
    private Transform targetPoint;
    private float moveSpeed;

    private AnimalSpawner spawner;
    private Lane lane;

    private bool isMoving;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
    }
    public void Initialize(
        Transform target,
        float speed,
        AnimalSpawner animalSpawner,
        Lane assignedLane, int layer)
    {
        targetPoint = target;
        moveSpeed = speed;

        spawner = animalSpawner;
        lane = assignedLane;
        spriteRenderer.sortingOrder = layer;
        isMoving = true;
    }

    private void Update()
    {
        if (!isMoving || targetPoint == null)
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

    public void ReturnToPool()
    {
        isMoving = false;

        lane.currentAnimals--;

        // spawner.AnimalRemoved(this);

        gameObject.SetActive(false);
    }
}