using System.Collections.Generic;
using UnityEngine;

public class StonePool : MonoBehaviour
{
    public static StonePool Instance { get; private set; }

    [SerializeField] private GameObject stonePrefab;
    [SerializeField] private int poolSize = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject stone = Instantiate(stonePrefab, transform);
            stone.SetActive(false);
            pool.Enqueue(stone);
        }
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        GameObject stone;

        if (pool.Count > 0)
        {
            stone = pool.Dequeue();
        }
        else
        {
            // Pool exhausted — grow it
            stone = Instantiate(stonePrefab, transform);
        }

        stone.transform.SetPositionAndRotation(position, rotation);
        stone.SetActive(true);

        Stone stoneComp = stone.GetComponent<Stone>();
        stoneComp?.ResetStone();

        return stone;
    }

    public void Return(GameObject stone)
    {
        stone.SetActive(false);
        pool.Enqueue(stone);
    }
}