using UnityEngine;

public class Aim : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject stonePrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float shootSpeed = 10f;
    private TrajectoryPredictor trajectoryPredictor;
    private Vector3 initialMousePos;
    private Vector2 direction;
    private float speed;
    void Awake()
    {
        trajectoryPredictor = GetComponent<TrajectoryPredictor>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            initialMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction = (mousePos - initialMousePos).normalized;
            speed = (mousePos - initialMousePos).magnitude;
            playerTransform.up = direction;
            trajectoryPredictor.ShowTrajectory(direction * shootSpeed * speed);
        }
        if (Input.GetMouseButtonUp(0))
        {
            Shoot(direction);
            trajectoryPredictor.HideTrajectory();
        }
        // Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Vector2 direction = (mousePos - playerTransform.position).normalized;
        // playerTransform.up = direction;
    }
    void Shoot(Vector2 direction)
    {
        GameObject stone = Instantiate(stonePrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = stone.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * shootSpeed * speed;
    }
}
