using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UFOLight : MonoBehaviour
{
    [SerializeField] private Vector3[] smallShapePoints;
    [SerializeField] private Vector3[] largeShapePoints;
    [SerializeField] private float speed;
    [SerializeField] private LayerMask animalLayer;
    [SerializeField] private float normalIntensity = 1f;
    [SerializeField] private float dimIntensity = 0.25f;
    [SerializeField] private float flickerDuration = 0.3f;

    private Coroutine flickerRoutine;
    private Light2D torch;
    private Vector3[] currentPath;
    private Vector3[] targetPath;
    private void Awake()
    {
        torch = GetComponent<Light2D>();

        currentPath = (Vector3[])smallShapePoints.Clone();

        targetPath = (Vector3[])largeShapePoints.Clone();

        torch.SetShapePath(currentPath);
    }
    private void OnEnable()
    {
        ResetLight();

        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, 10, animalLayer);

        if (hit)
        {
            StartCoroutine(SpreadTorchLight());
        }
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        ResetLight();
    }
    private IEnumerator SpreadTorchLight()
    {
        float timeElapsed = 0f;
        float totalDuration = speed;

        Vector3[] startPath =
            (Vector3[])smallShapePoints.Clone();

        while (timeElapsed < totalDuration)
        {
            timeElapsed += Time.deltaTime;

            float t = timeElapsed / totalDuration;

            t = Mathf.SmoothStep(0, 1, t);

            for (int i = 0; i < currentPath.Length; i++)
            {
                currentPath[i] = Vector3.Lerp(
                    startPath[i],
                    targetPath[i],
                    t);
            }

            torch.SetShapePath(currentPath);

            yield return null;
        }

        torch.SetShapePath(targetPath);
    }
    public void ResetLight()
    {
        StopAllCoroutines();
        // Optional
        flickerRoutine = null;

        currentPath = (Vector3[])smallShapePoints.Clone();
        targetPath = (Vector3[])largeShapePoints.Clone();

        torch.intensity = normalIntensity;
        torch.SetShapePath(currentPath);
    }
    public void PlayHitEffect()
    {
        if (flickerRoutine != null)
        {
            StopCoroutine(flickerRoutine);
        }

        flickerRoutine = StartCoroutine(FlickerRoutine());
    }
    private IEnumerator FlickerRoutine()
    {
        torch.intensity = 0f;

        yield return new WaitForSeconds(0.05f);

        torch.intensity = normalIntensity;

        yield return new WaitForSeconds(0.05f);

        torch.intensity = 0.2f;

        yield return new WaitForSeconds(0.08f);

        torch.intensity = normalIntensity;
    }
}
