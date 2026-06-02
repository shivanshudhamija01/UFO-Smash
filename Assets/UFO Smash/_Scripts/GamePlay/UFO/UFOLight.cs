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
    private Light2D torch;
    private Vector3[] currentPath;
    private Vector3[] targetPath;
    private void Awake()
    {
        torch = GetComponent<Light2D>();

        currentPath = (Vector3[])smallShapePoints.Clone();
        torch.SetShapePath(currentPath);
    }
   private void OnEnable()
    {
        ResetLight();

        RaycastHit2D hit =  Physics2D.Raycast(transform.position,-transform.up,10,animalLayer);

        if(hit)
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
        float t = 0;
        while (timeElapsed < totalDuration)
        {
            timeElapsed += Time.deltaTime;
            t = timeElapsed / totalDuration;

            t = Mathf.SmoothStep(0, 1, t);

            for (int i = 0; i < currentPath.Length; i++)
            {
                currentPath[i] = Vector3.Lerp(currentPath[i], targetPath[i], speed * Time.deltaTime);
            }
            torch.SetShapePath(currentPath);
            yield return null;
        }
        yield return null;
    }
    public void ResetLight()
    {
        StopAllCoroutines();

        currentPath = (Vector3[])smallShapePoints.Clone();
        targetPath = (Vector3[])largeShapePoints.Clone();

        torch.SetShapePath(currentPath);
    }
}
