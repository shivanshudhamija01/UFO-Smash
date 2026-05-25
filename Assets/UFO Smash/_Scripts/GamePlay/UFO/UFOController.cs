using System;
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
    private int currentHealth;

    private UFOStateMachine stateMachine;

    private void Awake()
    {
        stateMachine =
            new UFOStateMachine(this);
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
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Stone"))
            return;

        if (stateMachine.GetCurrentState() != UFOStates.abduct)
            return;

        Stone stone = collision.gameObject.GetComponent<Stone>();

        int damage = stone != null ? stone.GetDamage() : 1;

        TakeDamage(damage);
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.fillAmount = (float)currentHealth / maxHealth;
        Debug.Log($"{gameObject.name} HP: {currentHealth}");

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
        stateMachine.ChangeState(UFOStates.blast);
    }
    public void FinishUFO()
    {
        torchLight.gameObject.SetActive(false);
        OnUFOFinished?.Invoke(this);
    }
    // Getters
    public Transform GetTransform() => transform;
    public Light2D GetTorchLight() => torchLight;
    public UFOType GetUFOType() => uFOType;
    public SplineContainer GetSpline() => splineContainer;
    public float GetSplineSpeed() => splineMoveSpeed;
    public bool ShouldRotateSpline() => rotateAlongSpline;

    public float GetManualSpeed() => manualMoveSpeed;
    public Vector2 GetOffset() => offset;

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

// I got it why the UFO movement is not working , because the initialize is called in Spawning script of both the animal and UFO controller
