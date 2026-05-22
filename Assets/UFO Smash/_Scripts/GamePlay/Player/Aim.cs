using UnityEngine;

public class Aim : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject stonePrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float shootSpeed = 10f;

    [Header("Stone Ammo")]
    [SerializeField] private int maxStoneCount = 5;

    private int currentStoneCount;

    private TrajectoryPredictor trajectoryPredictor;
    private Vector3 initialMousePos;
    private Vector2 direction;
    private float speed;

    private bool canShoot = true;

    void Awake()
    {
        trajectoryPredictor = GetComponent<TrajectoryPredictor>();
    }

    void Start()
    {
        currentStoneCount = maxStoneCount;
    }

    void Update()
    {
        if (!canShoot) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (currentStoneCount <= 0) return;

            initialMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            if (currentStoneCount <= 0) return;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            direction = (mousePos - initialMousePos).normalized;

            speed = (mousePos - initialMousePos).magnitude;

            trajectoryPredictor.ShowTrajectory(direction * shootSpeed * speed);
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (currentStoneCount <= 0) return;

            Shoot(direction);
            trajectoryPredictor.HideTrajectory();
        }
    }

    void Shoot(Vector2 direction)
    {
        GameObject stone = Instantiate(stonePrefab, firePoint.position, firePoint.rotation);

        Rigidbody2D rb = stone.GetComponent<Rigidbody2D>();

        rb.linearVelocity = direction * shootSpeed * speed;

        currentStoneCount--;
        Debug.Log("Stone Left: " + currentStoneCount);
    }

    public void ReloadToMax()
    {
        currentStoneCount = maxStoneCount;

        Debug.Log("Stones Reloaded!");
    }

    public int GetCurrentAmmo()
    {
        return currentStoneCount;
    }

    public int GetMaxAmmo()
    {
        return maxStoneCount;
    }

    // if want to add gradual refilling
    // public void AddStone(int amount = 1)
    // {
    //     currentStoneCount += amount;

    //     currentStoneCount =
    //         Mathf.Clamp(currentStoneCount, 0, maxStoneCount);

    //     Debug.Log("Stone Count: " + currentStoneCount);
    // }
}