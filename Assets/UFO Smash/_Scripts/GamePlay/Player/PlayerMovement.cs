using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Animator animator;

    [Header("Clamp Settings")]
    [SerializeField] private float padding = 0.5f;

    private readonly int isWalkingHash =
        Animator.StringToHash("IsWalking");

    private IEventBus eventBus;
    private int moveDirection;

    private float minX;
    private float maxX;
    private bool isGamePaused = false;

    private void Awake()
    {
        eventBus = ServiceLocator.Get<IEventBus>();
    }

    private void Start()
    {
        CalculateScreenBounds();
    }

    private void OnEnable()
    {
        eventBus.Add<Events.OnGameReset>(OnGameReset);
        eventBus.Add<Events.OnGameInput>(HandleInput);
        eventBus.Add<Events.DisableGameplayInput>(DisableMovement);
        eventBus.Add<Events.OnGameStarted>(OnGameStart);
    }

    private void OnDisable()
    {
        eventBus.Remove<Events.OnGameReset>(OnGameReset);
        eventBus.Remove<Events.OnGameInput>(HandleInput);
        eventBus.Remove<Events.DisableGameplayInput>(DisableMovement);
        eventBus.Remove<Events.OnGameStarted>(OnGameStart);
    }

    private void HandleInput(Events.OnGameInput evt)
    {
        moveDirection = evt.Direction;
    }

    private void Update()
    {
        if (isGamePaused)
        {
            return;
        }

        bool isMoving = moveDirection != 0;

        transform.Translate(
            Vector3.right *
            moveDirection *
            speed *
            Time.deltaTime);

        ClampPosition();

        animator.SetBool(
            isWalkingHash,
            isMoving);
    }

    private void CalculateScreenBounds()
    {
        Camera cam = Camera.main;

        Vector3 left =
            cam.ViewportToWorldPoint(
                new Vector3(0f, 0.5f, cam.nearClipPlane));

        Vector3 right =
            cam.ViewportToWorldPoint(
                new Vector3(1f, 0.5f, cam.nearClipPlane));

        minX = left.x + padding;
        maxX = right.x - padding;
    }

    private void ClampPosition()
    {
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(
            pos.x,
            minX,
            maxX);

        transform.position = pos;
    }
    private void DisableMovement(Events.DisableGameplayInput evt)
    {
        moveDirection = 0;
        isGamePaused = true;
    }
    private void OnGameReset(Events.OnGameReset evt)
    {
        moveDirection = 0;
        isGamePaused = false;
    }
    private void OnGameStart(Events.OnGameStarted evt)
    {
        isGamePaused = false;
    }
}