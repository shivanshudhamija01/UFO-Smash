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
        currentPath = smallShapePoints;
        targetPath = largeShapePoints;
        torch = GetComponent<Light2D>();
        torch.SetShapePath(currentPath);
    }
    private void OnEnable()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, 10, animalLayer);
        if (hit)
        {
            StartCoroutine(SpreadTorchLight());
        }
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

}
