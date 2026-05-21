using UnityEngine;

public class UFOSuccess
    : BaseState<UFOController>
{
    private Transform transform;
    private Vector3 startScale;
    private bool startEscape;
    private float initialTilt = 45f;
    private float finalTilt = -45f;
    private float tiltSpeed = 3f;
    private float escapeSpeed = 7f;

    private float shrinkSpeed = 1f;

    private float anticipationTimer;
    private float anticipationDuration = 0.5f;

    // Fixed top-left direction
    private Vector2 escapeDirection =
        new Vector2(-1f, 1f).normalized;

    public UFOSuccess(UFOController controller) : base(controller)
    {
    }

    public override void OnEnterState()
    {
        transform = controller.GetTransform();

        startScale = transform.localScale;

        startEscape = false;

        anticipationTimer = 0f;

        // Debug.Log("Entered into UFO Success state");
    }

    public override void UpdateState()
    {
        // Phase 1:
        // Small left anticipation tilt
        if (!startEscape)
        {
            AnticipationTilt();
            return;
        }

        // // Phase 2:
        EscapeMovement();
        RotateAndShrink();
    }

    public override void OnExitState()
    {
        transform.rotation = Quaternion.identity;

        transform.localScale = startScale;

    }

    public override void FixedUpdateState()
    {
    }

    private void AnticipationTilt()
    {
        anticipationTimer += Time.deltaTime;

        Quaternion targetRotation = Quaternion.Euler(0, 0, initialTilt);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, tiltSpeed * Time.deltaTime);

        if (anticipationTimer >= anticipationDuration)
        {
            startEscape = true;
        }
    }

    private void EscapeMovement()
    {
        transform.position += (Vector3)(escapeDirection * escapeSpeed * Time.deltaTime);
    }

    private void RotateAndShrink()
    {
        // Rotate toward right
        Quaternion targetRotation = Quaternion.Euler(0, 0, finalTilt);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, tiltSpeed * Time.deltaTime);

        // Shrink while escaping
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, shrinkSpeed * Time.deltaTime);

        // Disable when tiny
        if (transform.localScale.x <= 0.05f)
        {
            // controller.gameObject.SetActive(false);
            controller.FinishUFO();
        }
    }
}