using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Splines;

public class UFO : MonoBehaviour
{
    [Header("Current State")]
    public UFOState currentState;

    [Header("Spline Settings")]
    public SplineContainer splineContainer;

    public float splineMoveSpeed = 5f;

    public bool rotateAlongSpline = true;

    private float splineDistance;

    private float splineLength;

    [Header("Manual Movement Settings")]
    public float manualMoveSpeed = 3f;

    public Vector2 moveDirection = Vector2.right;

    [SerializeField] private Light2D UFOTorchLight;
    [SerializeField] private Transform lockedAnimal;
    [SerializeField] private float tiltSpeed;
    [SerializeField] private Vector2 offset;

    private Vector2 targetAnimal;
    private float t = 0;
    private void Start()
    {
        currentState = UFOState.SplineMovement;

        splineLength = splineContainer.CalculateLength();
    }

    private void Update()
    {
        switch (currentState)
        {
            case UFOState.SplineMovement:
                FollowSpline();
                break;

                // case UFOState.ManualMovement:
                //     ManualMovement();
                //     break;
        }
    }

    void FollowSpline()
    {
        splineDistance += splineMoveSpeed * Time.deltaTime;

        float normalizedDistance =
            splineDistance / splineLength;

        normalizedDistance =
            Mathf.Clamp01(normalizedDistance);

        // Move UFO
        Vector3 position =
            splineContainer.EvaluatePosition(normalizedDistance);

        // Here what i can try is that, when the UFO reached the 0.5f of the spline length 
        // then it will start lerping towards the offset position of the walking animal 
        transform.position = position;

        // Rotate UFO in 2D
        if (rotateAlongSpline)
        {
            Vector3 tangent =
                splineContainer.EvaluateTangent(normalizedDistance);

            float angle =
                Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;

            transform.rotation =
                Quaternion.Euler(0, 0, angle + 180f);
        }
        // Reached end of spline
        if (normalizedDistance >= 0.75f)
        {
            SwitchToManualMovement();
        }
    }

    void SwitchToManualMovement()
    {
        t = 0;

        currentState = UFOState.ManualMovement;
        ManualMovement();
    }

    void ManualMovement()
    {
        Vector2 direction = (lockedAnimal.position - transform.position).normalized;
        if (direction.x < 0)
        {
            // Debug.Log("Means animal is present to the left of the UFO");
        }
        else
        {
            // Debug.Log("Means animal is present to the right of the UFO");
        }
        int dir = direction.x < 0 ? -1 : 1;
        float angle = dir * 30;
        Vector2 shiftValue = new Vector2(dir * offset.x, offset.y);


        StartCoroutine(UFOIntroMovement(shiftValue, angle));
    }

    private IEnumerator UFOIntroMovement(Vector2 shiftValue, float targetAngle)
    {
        yield return StartCoroutine(OverShootAndTiltWithJerk(shiftValue.x, shiftValue.y, targetAngle));
        yield return new WaitForSeconds(0.15f);
        yield return StartCoroutine(OverShootAndTiltWithJerk(-shiftValue.x, shiftValue.y, -targetAngle));
        manualMoveSpeed = 0.7f;
        yield return StartCoroutine(OverShootAndTilt(shiftValue.x / 2, shiftValue.y, targetAngle / 2));
        yield return StartCoroutine(OverShootAndTilt(-shiftValue.x / 2, shiftValue.y, -targetAngle / 2));
        yield return StartCoroutine(MoveToAnimal());

    }
    private IEnumerator OverShootAndTiltWithJerk(float x, float y, float targetAngle)
    {
        // Debug.Log("OverShootAndTiltWithJerk is called");
        Vector2 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Vector2 targetPos = new Vector2(lockedAnimal.position.x + x, lockedAnimal.position.y + y);

        Quaternion targetRot = Quaternion.Euler(0, 0, targetAngle);

        float duration = manualMoveSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;

            transform.position = Vector2.Lerp(startPos, targetPos, t);

            if (t > 0.25f)
            {
                t = Mathf.SmoothStep(0, 1, t);
                transform.rotation = Quaternion.Lerp(startRot, targetRot, t);
            }

            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
    }
    private IEnumerator OverShootAndTilt(float x, float y, float targetAngle)
    {
        // Debug.Log("OverShootAndTilt is called");
        AnimalMotion animal = lockedAnimal.gameObject.GetComponent<AnimalMotion>();
        animal.SetAbduct();
        Vector2 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Vector2 targetPos = new Vector2(lockedAnimal.position.x + x, lockedAnimal.position.y + y);

        Quaternion targetRot = Quaternion.Euler(0, 0, targetAngle);

        float duration = manualMoveSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;

            t = Mathf.SmoothStep(0, 1, t);

            transform.position = Vector2.Lerp(startPos, targetPos, t);

            // Rotation interpolation
            transform.rotation = Quaternion.Lerp(startRot, targetRot, t);

            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;
    }
    private IEnumerator MoveToAnimal()
    {
        // Debug.Log("Move To Animal");
        Vector2 startPos = transform.position;
        Vector2 targetPos = new Vector2(lockedAnimal.position.x, lockedAnimal.position.y + offset.y);
        Quaternion targetRot = Quaternion.Euler(0, 0, 0);
        float duration = 0.4f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;

            t = Mathf.SmoothStep(0, 1, t);

            transform.position = Vector2.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, t);

            yield return null;
        }

        transform.position = targetPos;
        UFOTorchLight.gameObject.SetActive(true);
    }
}
