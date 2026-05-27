using System;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Splines;
using UnityEngine.UI;

public class UFOController : MonoBehaviour
{
    public static event Action<UFOController> OnUFOFinished;
    [Header("Spline")]
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private float splineMoveSpeed = 5f;
    [SerializeField] private bool rotateAlongSpline = true;

    [Header("Movement")]
    [SerializeField] private float manualMoveSpeed = 3f;
    [SerializeField] private Vector2 offset;
    [SerializeField] private float tiltSpeed;

    [Header("References")]
    [SerializeField] private Light2D torchLight;
    [SerializeField] private IAbductable lockedAnimal;
    [Header("UFO Type")]
    [SerializeField] private UFOType uFOType;
    [Header("UFO Health")]
    [SerializeField] private int maxHealth;
    [SerializeField] private Image healthBar;
    [SerializeField] private Canvas healthBarCanvas;
    [Header("UFO Score Value")]
    [SerializeField] private int scoreValue;
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [Header("Hit Shake")]
    [SerializeField] private float hitShakeDuration = 0.12f;
    [SerializeField] private float hitShakeStrength = 0.08f;
    [SerializeField] private float hitShakeSpeed = 45f;
    private readonly int key = Animator.StringToHash("IsSpline");
    private bool isShaking;

    private float shakeTimer;

    private Vector3 originalPosition;
    private int currentHealth;
    private IScoreService scoreService;
    private UFOStateMachine stateMachine;
    private IEventBus eventBus;
    private void Awake()
    {
        scoreService = ServiceLocator.Get<IScoreService>();
        eventBus = ServiceLocator.Get<IEventBus>();
        stateMachine = new UFOStateMachine(this);
    }
    // Remove this start method later and call the initialize method in spawning
    private void Start()
    {
        // Initialize(lockedAnimal);
        // Initialize();
    }
    // public void Initialize(IAbductable targetAnimal)
    // {
    //     lockedAnimal = targetAnimal;

    //     stateMachine.ChangeState(
    //         UFOStates.spline
    //     );
    // }
    public void Initialize(SplineContainer spline)
    {
        splineContainer = spline;
        currentHealth = maxHealth;
        healthBar.fillAmount = 1;
        healthBarCanvas.gameObject.SetActive(false);
        animator.SetBool(key, true);
        if (splineContainer != null)
        {
            Vector3 startPos = splineContainer.EvaluatePosition(0f);
            transform.position = startPos;

            if (rotateAlongSpline)
            {
                Vector3 tangent = splineContainer.EvaluateTangent(0f);
                float angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
        stateMachine.ChangeState(UFOStates.spline);
    }
    private void Update()
    {
        stateMachine.Update();
        HandleHitShake();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Stone"))
            return;

        if (stateMachine.GetCurrentState() != UFOStates.abduct)
            return;

        Stone stone = collision.gameObject.GetComponent<Stone>();

        // Here i need to check whether it has damaged or not 
        int damage = stone != null ? stone.GetDamage() : 1;
        if (!stone.HasAlreadyHitUFO())
        {
            TakeDamage(damage);
        }
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.fillAmount = (float)currentHealth / maxHealth;
        StartHitShake();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (lockedAnimal != null)
        {
            lockedAnimal.ReleaseFromAbduction();
            lockedAnimal = null;
        }
        scoreService.AddScore(scoreValue);
        eventBus.Publish(new Events.OnUFODestroyed());
        stateMachine.ChangeState(UFOStates.blast);
    }
    public void FinishUFO()
    {
        animator.SetBool(key, false);
        torchLight.gameObject.SetActive(false);
        OnUFOFinished?.Invoke(this);
    }

    #region UFO - Shake
    private void StartHitShake()
    {
        if (stateMachine.GetCurrentState() == UFOStates.blast)
            return;

        shakeTimer = hitShakeDuration;
        isShaking = true;
        originalPosition = transform.position;
    }

    private void HandleHitShake()
    {
        if (!isShaking)
            return;

        shakeTimer -= Time.deltaTime;

        if (shakeTimer <= 0)
        {
            isShaking = false;
            transform.position = originalPosition;
            return;
        }

        float progress = shakeTimer / hitShakeDuration;

        float noiseX = (Mathf.PerlinNoise(Time.time * hitShakeSpeed, 0f) - 0.5f) * 2f;
        float noiseY = (Mathf.PerlinNoise(0f, Time.time * hitShakeSpeed) - 0.5f) * 2f;

        // Multiply by progress so shake eases out naturally
        float xOffset = noiseX * hitShakeStrength * progress;
        float yOffset = noiseY * hitShakeStrength * progress;

        transform.position = originalPosition + new Vector3(xOffset, yOffset, 0f);
    }
    #endregion
    // Getters
    public Transform GetTransform() => transform;
    public Light2D GetTorchLight() => torchLight;
    public UFOType GetUFOType() => uFOType;
    public SplineContainer GetSpline() => splineContainer;
    public float GetSplineSpeed() => splineMoveSpeed;
    public bool ShouldRotateSpline() => rotateAlongSpline;

    public float GetManualSpeed() => manualMoveSpeed;
    public Vector2 GetOffset() => offset;
    public Animator GetAnimator() => animator;
    public UFOStateMachine GetStateMachine()
        => stateMachine;

    public Canvas GetHealthBar() => healthBarCanvas;
    public void SetLockedAnimal(IAbductable animal)
    {
        lockedAnimal = animal;
    }

    public IAbductable GetLockedAnimal()
    {
        return lockedAnimal;
    }
}

// Now try to play the idle animation in loop
