using UnityEngine;

public class Aim : MonoBehaviour
{
    [SerializeField]
    private Transform firePoint;

    [SerializeField]
    private GameObject stonePrefab;

    [SerializeField]
    private Transform rightHandTransform;

    [SerializeField]
    private float shootSpeed = 10f;

    [SerializeField]
    private float maxAimRange = 10f;

    [Header("Aim Restriction")]
    [SerializeField]
    private float minAimAngle = -70f;

    [SerializeField]
    private float maxAimAngle = 70f;

    [SerializeField]
    private float handRotationOffset = -90f;

    [Header("Stone Ammo")]
    [SerializeField]
    private int maxStoneCount = 5;

    private int currentStoneCount;

    private TrajectoryPredictor
        trajectoryPredictor;

    private Vector3 initialMousePos;

    private Vector2 direction;

    private float speed;

    private bool canShoot = true;

    private void Awake()
    {
        trajectoryPredictor = GetComponent<TrajectoryPredictor>();
    }

    private void Start()
    {
        currentStoneCount = maxStoneCount;
    }

    private void Update()
    {
        if (!canShoot)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (currentStoneCount <= 0)
                return;

            initialMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            if (currentStoneCount <= 0)
                return;

            HandleAim();
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (currentStoneCount <= 0)
                return;

            Shoot(direction);

            trajectoryPredictor.HideTrajectory();
        }
    }

    private void HandleAim()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 rawDirection = mousePos - firePoint.position;

        // Calculate angle
        float angle = Mathf.Atan2(rawDirection.y, rawDirection.x) * Mathf.Rad2Deg;

        // Clamp angle
        angle = Mathf.Clamp(angle, minAimAngle, maxAimAngle);

        // Convert back to direction
        direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;

        // Speed
        speed = Vector2.Distance(mousePos, initialMousePos);

        speed = Mathf.Clamp(speed, 0, maxAimRange);

        // Rotate hand
        RotateHand(angle);

        trajectoryPredictor.ShowTrajectory(direction * shootSpeed * speed);
    }

    private void RotateHand(float angle)
    {
        rightHandTransform.rotation = Quaternion.Euler(0, 180, angle + handRotationOffset);
    }

    private void Shoot(Vector2 direction)
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
}