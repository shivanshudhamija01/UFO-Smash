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
    private VariableJoystick variableJoystick;
    private bool wasDraggingJoystick;
    private IAudioService audioService;

    private void Awake()
    {
        trajectoryPredictor = GetComponent<TrajectoryPredictor>();
        eventBus = ServiceLocator.GetService<IEventBus>();
        audioService = ServiceLocator.GetService<IAudioService>();
    }

    private void Start()
    {
        currentStoneCount = maxStoneCount;
        eventBus.Publish(new Events.OnStoneReloaded(currentStoneCount));
    }
    private void OnEnable()
    {
        eventBus.Add<Events.OnGameReset>(ResetStoneCount);
    }
    private void OnDisable()
    {
        eventBus.Remove<Events.OnGameReset>(ResetStoneCount);
    }
    private void Update()
    {
        if (!canShoot)
            return;

        Vector2 joystickInput = new Vector2(
            variableJoystick.Horizontal,
            variableJoystick.Vertical);

        bool isDragging = joystickInput.magnitude > 0.1f;

        if (isDragging)
        {
            if (currentStoneCount <= 0)
            {
                ResetHandToRest();
                return;
            }

            stoneVisuals.SetActive(true);
            HandleAim();
        }

        // Shoot when joystick is released
        if (wasDraggingJoystick && !isDragging)
        {
            if (currentStoneCount > 0)
            {
                Shoot(direction);
            }

            stoneVisuals.SetActive(false);
            trajectoryPredictor.HideTrajectory();
        }

        wasDraggingJoystick = isDragging;
    }

    private void HandleAim()
    {
        Vector2 rawDirection = new Vector2(
            variableJoystick.Horizontal,
            variableJoystick.Vertical);

        if (rawDirection.sqrMagnitude < 0.01f)
            return;

        float angle =
            Mathf.Atan2(rawDirection.y, rawDirection.x) * Mathf.Rad2Deg;

        angle = Mathf.Clamp(angle, minAimAngle, maxAimAngle);

        currentAimAngle = angle;

        direction = new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad))
            .normalized;

        // Power based on joystick distance from center
        speed = Mathf.Clamp(rawDirection.magnitude, 0f, 1f) * maxAimRange;

        RotateHand(angle);

        trajectoryPredictor.ShowTrajectory(
            direction * shootSpeed * speed);
    }

    private void RotateHand(float angle)
    {
        rightHandTransform.rotation = Quaternion.Euler(0, 180, -angle + handRotationOffset);
    }

    private void Shoot(Vector2 direction)
    {
        if (speed <= 0.05f)
            return;

        GameObject stone = StonePool.Instance.Get(firePoint.position, firePoint.rotation);
        Rigidbody2D rb = stone.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * shootSpeed * speed;
        audioService.SFX(SoundType.StoneThrow);
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
    private void ResetStoneCount(Events.OnGameReset evt)
    {
        currentStoneCount = maxStoneCount;

        eventBus.Publish(new Events.OnStoneReloaded(currentStoneCount));
        direction = Vector2.zero;

        wasDraggingJoystick = false;

        variableJoystick.ResetJoystick();

        trajectoryPredictor.HideTrajectory();

        stoneVisuals.SetActive(false);
    }

    public int GetCurrentAmmo() => currentStoneCount;
    public int GetMaxAmmo() => maxStoneCount;
    public void SetVariableJoystick(VariableJoystick value) => variableJoystick = value;
}
// I have to take the reference of the joystick to the player 