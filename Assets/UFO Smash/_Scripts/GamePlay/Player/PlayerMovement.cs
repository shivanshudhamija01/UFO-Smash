using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Animator animator;

    private readonly int isWalkingHash = Animator.StringToHash("IsWalking");

    private IEventBus eventBus;
    private int moveDirection;

    private void Awake()
    {
        eventBus = ServiceLocator.Get<IEventBus>();
    }

    private void OnEnable()
    {
        eventBus.Add<Events.OnGameInput>(HandleInput);
    }

    private void OnDisable()
    {
        eventBus.Remove<Events.OnGameInput>(HandleInput);
    }

    private void HandleInput(Events.OnGameInput evt)
    {
        moveDirection = evt.Direction;
    }

    private void Update()
    {
        bool isMoving = moveDirection != 0;

        transform.Translate(Vector3.right * moveDirection * speed * Time.deltaTime);

        animator.SetBool(isWalkingHash, isMoving);
    }
}