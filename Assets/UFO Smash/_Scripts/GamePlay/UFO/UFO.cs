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
        // // Here i need to lerp towards the targetted animal and then need
        // // lerp towards it in terms of rotation and transform a little ahead and then little back and then follow it and turn on the light
        // t = manualMoveSpeed * Time.deltaTime;

        // transform.position = Vector2.Lerp(transform.position, targetAnimal, t);
        // // transform.rotation = Vector3.Lerp(transform.rotation,new Quaternion.(0,0,45f),)

        Vector2 direction = (lockedAnimal.position - transform.position).normalized;
        if (direction.x < 0)
        {
            Debug.Log("Means animal is present to the left of the UFO");
        }
        else
        {
            Debug.Log("Means animal is present to the right of the UFO");
        }
        int dir = direction.x < 0 ? -1 : 1;
        float angle = dir * 30;
        Vector2 shiftValue = new Vector2(dir * offset.x, offset.y);


        StartCoroutine(UFOIntroMovement(shiftValue, angle));
    }
    // in this , i am going to pass the direction of tilt and movement, 
    // i will call this coroutine in two different direction . 
    // then i will call a another method that will reach the exact position of animal head and then
    // then spawn the light , and after locking an animal , it start shivering 


    private IEnumerator UFOIntroMovement(Vector2 shiftValue, float targetAngle)
    {
        yield return StartCoroutine(OverShootAndTiltWithJerk(shiftValue.x, shiftValue.y, targetAngle));
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(OverShootAndTiltWithJerk(-shiftValue.x, shiftValue.y, -targetAngle));
        yield return StartCoroutine(OverShootAndTilt(shiftValue.x / 2, shiftValue.y, targetAngle / 2));
        yield return StartCoroutine(OverShootAndTilt(-shiftValue.x / 2, shiftValue.y, -targetAngle / 2));
        yield return StartCoroutine(MoveToAnimal());

    }
    private IEnumerator OverShootAndTiltWithJerk(float x, float y, float targetAngle)
    {
        Debug.Log("OverShootAndTiltWithJerk is called");
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

            // Smooth easing

            // Position interpolation
            transform.position = Vector2.Lerp(startPos, targetPos, t);

            // Rotation interpolation
            if (t > 0.35f)
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
        Debug.Log("OverShootAndTilt is called");
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

            // Smooth easing
            t = Mathf.SmoothStep(0, 1, t);

            // Position interpolation
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
        Debug.Log("Move To Animal");
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
