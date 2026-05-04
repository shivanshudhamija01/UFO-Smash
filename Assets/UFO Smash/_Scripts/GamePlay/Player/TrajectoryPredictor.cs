using UnityEngine;

public class TrajectoryPredictor : MonoBehaviour
{
    [SerializeField] private GameObject trajectoryPointPrefab;
    [SerializeField] private int numberOfPoints = 30;
    [SerializeField] private float timeStep = 0.1f;
    [SerializeField] private Transform launchPoint;

    private GameObject[] trajectoryPoints;
    private void Start()
    {
        trajectoryPoints = new GameObject[numberOfPoints];
        for(int i = 0;i<numberOfPoints;i++)
        {
            trajectoryPoints[i]=Instantiate(trajectoryPointPrefab, launchPoint.position, Quaternion.identity);
            trajectoryPoints[i].SetActive(false);
        }
    }

    public void ShowTrajectory(Vector2 initialVelocity)
    {
        for(int i = 0;i<numberOfPoints;i++)
        {
            float t = i* timeStep;
            Vector2 position = (Vector2)launchPoint.position + initialVelocity * t + 0.5f * Physics2D.gravity * t * t;
            trajectoryPoints[i].transform.position = position;
            trajectoryPoints[i].SetActive(true);
        }
    }
    public void HideTrajectory()
    {
        for(int i = 0;i<numberOfPoints;i++)
        {
            trajectoryPoints[i].SetActive(false);
        }
    }
}
