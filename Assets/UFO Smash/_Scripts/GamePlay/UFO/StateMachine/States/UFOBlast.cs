using UnityEngine;

public class UFOBlast
    : BaseState<UFOController>
{
    private Transform transform;

    // Phase control
    private bool startFalling;

    // Hit reaction
    private float hitTimer;
    private float hitDuration = 0.25f;

    private float oscillateSpeed = 30f;
    private float oscillateAmount = 12f;

    // Falling
    private Vector2 fallDirection;

    private float fallSpeed = 0f;
    private float maxFallSpeed = 10f;
    private float acceleration = 15f;

    private float spinSpeed = 450f;

    public UFOBlast(
        UFOController controller)
        : base(controller)
    {
    }

    public override void OnEnterState()
    {
        transform = controller.GetTransform();

        startFalling = false;

        hitTimer = 0f;

        // Random side drift
        float xDirection = Random.Range(-0.5f, 0.5f);

        fallDirection = new Vector2(xDirection, -1f).normalized;

        // Debug.Log("UFO Hit by Stone!");
    }

    public override void UpdateState()
    {
        if (!startFalling)
        {
            HitReaction();
            return;
        }

        FallAndSpin();
    }

    public override void OnExitState()
    {
        transform.rotation = Quaternion.identity;
    }

    public override void FixedUpdateState()
    {
    }

    private void HitReaction()
    {
        hitTimer += Time.deltaTime;

        // Quick oscillation
        float angle = Mathf.Sin(Time.time * oscillateSpeed) * oscillateAmount;

        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (hitTimer >= hitDuration)
        {
            startFalling = true;
        }
    }

    private void FallAndSpin()
    {
        // Gravity feel
        fallSpeed += acceleration * Time.deltaTime;

        fallSpeed = Mathf.Min(fallSpeed, maxFallSpeed);

        // Move downward
        transform.position += (Vector3)(fallDirection * fallSpeed * Time.deltaTime);

        // Spin uncontrollably
        transform.Rotate(0, 0, spinSpeed * Time.deltaTime);

        // Ground check
        if (transform.position.y <= -5f)
        {
            Blast();
        }
    }

    private void Blast()
    {
        // Debug.Log("BOOM!");

        // TODO:
        // Spawn blast VFX

        // controller.gameObject.SetActive(false);
        controller.FinishUFO();
    }
}