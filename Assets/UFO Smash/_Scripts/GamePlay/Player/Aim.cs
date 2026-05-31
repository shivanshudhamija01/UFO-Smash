using UnityEngine;
using System.Collections;

public class Aim : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform rightHandTransform;
    [SerializeField] private float shootSpeed = 10f;
    [SerializeField] private float maxAimRange = 10f;

    [Header("Aim Restriction")]
    [SerializeField] private float minAimAngle = -70f;
    [SerializeField] private float maxAimAngle = 70f;
    [SerializeField] private float handRotationOffset = -90f;

    [Header("Throw Animation")]
    [SerializeField] private float throwPunchAngle = 40f;
    [SerializeField] private float throwPunchSpeed = 15f;
    [SerializeField] private float throwReturnSpeed = 8f;
    [SerializeField] private float resetDuration = 0.4f;

    [Header("Stone Ammo")]
    [SerializeField] private int maxStoneCount = 5;
    [SerializeField] private GameObject stoneVisuals;

    private int currentStoneCount;
    private TrajectoryPredictor trajectoryPredictor;
    private Vector3 initialMousePos;
    private Vector2 direction;
    private float speed;
    private bool canShoot = true;
    private float currentAimAngle = 0f;
    private Coroutine throwAnimCoroutine;
    private Coroutine resetHandCoroutine;
    private IEventBus eventBus;

    private void Awake()
    {
        trajectoryPredictor = GetComponent<TrajectoryPredictor>();
        eventBus = ServiceLocator.Get<IEventBus>();
    }

    private void Start()
    {
        currentStoneCount = maxStoneCount;
        eventBus.Publish(new Events.OnStoneReloaded(currentStoneCount));
    }

    private void Update()
    {
        if (!canShoot)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (currentStoneCount <= 0)
            {
                ResetHandToRest();
                return;
            }

            initialMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            stoneVisuals.SetActive(true);
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
            stoneVisuals.SetActive(false);
            trajectoryPredictor.HideTrajectory();
        }
    }

    private void HandleAim()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 rawDirection = mousePos - firePoint.position;

        float angle = Mathf.Atan2(rawDirection.y, rawDirection.x) * Mathf.Rad2Deg;
        angle = Mathf.Clamp(angle, minAimAngle, maxAimAngle);

        currentAimAngle = angle;

        direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;

        speed = Mathf.Clamp(Vector2.Distance(mousePos, initialMousePos), 0, maxAimRange);

        RotateHand(angle);

        trajectoryPredictor.ShowTrajectory(direction * shootSpeed * speed);
    }

    private void RotateHand(float angle)
    {
        rightHandTransform.rotation = Quaternion.Euler(0, 180, -angle + handRotationOffset);
    }

    private void Shoot(Vector2 direction)
    {
        GameObject stone = StonePool.Instance.Get(firePoint.position, firePoint.rotation);
        Rigidbody2D rb = stone.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * shootSpeed * speed;

        currentStoneCount--;
        eventBus.Publish(new Events.OnStoneShot(currentStoneCount));

        if (throwAnimCoroutine != null)
            StopCoroutine(throwAnimCoroutine);

        throwAnimCoroutine = StartCoroutine(ThrowAnimation());

        if (currentStoneCount <= 0)
            StartCoroutine(ResetAfterAnimation());
    }

    private IEnumerator ThrowAnimation()
    {
        canShoot = false;

        float currentZ = -currentAimAngle + handRotationOffset;
        float startX = 0f;
        float punchX = -throwPunchAngle;

        // Phase 1: punch towards screen
        float current = startX;
        while (Mathf.Abs(current - punchX) > 0.5f)
        {
            current = Mathf.MoveTowards(current, punchX, throwPunchSpeed * Time.deltaTime * 60f);
            rightHandTransform.rotation = Quaternion.Euler(current, 180, currentZ);
            yield return null;
        }

        // Phase 2: return back
        current = punchX;
        while (Mathf.Abs(current - startX) > 0.5f)
        {
            current = Mathf.MoveTowards(current, startX, throwReturnSpeed * Time.deltaTime * 60f);
            rightHandTransform.rotation = Quaternion.Euler(current, 180, currentZ);
            yield return null;
        }

        rightHandTransform.rotation = Quaternion.Euler(startX, 180, currentZ);
        canShoot = true;
        throwAnimCoroutine = null;
        ResetHandToRest();

    }

    private IEnumerator ResetAfterAnimation()
    {
        yield return new WaitUntil(() => throwAnimCoroutine == null);
        ResetHandToRest();
    }

    private void ResetHandToRest()
    {
        if (throwAnimCoroutine != null)
        {
            StopCoroutine(throwAnimCoroutine);
            throwAnimCoroutine = null;
        }

        if (resetHandCoroutine != null)
            StopCoroutine(resetHandCoroutine);

        resetHandCoroutine = StartCoroutine(ResetHandAnimation());
    }

    private IEnumerator ResetHandAnimation()
    {
        Quaternion startRotation = rightHandTransform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, 20f);

        float elapsed = 0f;

        while (elapsed < resetDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / resetDuration);
            rightHandTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }

        rightHandTransform.rotation = targetRotation;
        resetHandCoroutine = null;
    }

    public void ReloadToMax()
    {
        currentStoneCount = maxStoneCount;
        eventBus.Publish(new Events.OnStoneReloaded(currentStoneCount));
        ResetHandToRest();
    }

    public int GetCurrentAmmo() => currentStoneCount;
    public int GetMaxAmmo() => maxStoneCount;
}