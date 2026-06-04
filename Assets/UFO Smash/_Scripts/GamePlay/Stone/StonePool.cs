using System.Collections.Generic;
using UnityEngine;

public class StonePool : MonoBehaviour
{
    public static StonePool Instance { get; private set; }

    [SerializeField] private GameObject stonePrefab;
    [SerializeField] private int poolSize = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();
    private List<GameObject> activeStones = new List<GameObject>();
    private IEventBus eventBus;

    private void Awake()
    {
        Instance = this;
        eventBus = ServiceLocator.GetService<IEventBus>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject stone = Instantiate(stonePrefab, transform);
            stone.SetActive(false);
            pool.Enqueue(stone);
        }
    }
    private void OnEnable()
    {
        eventBus.Add<Events.OnGameReset>(HandleReset);
    }
    private void OnDisable()
    {
        eventBus.Remove<Events.OnGameReset>(HandleReset);
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
        activeStones.Add(stone);
        Stone stoneComp = stone.GetComponent<Stone>();
        stoneComp?.ResetStone();

        return stone;
    }

    public void Return(GameObject stone)
    {
        if (activeStones.Contains(stone))
        {
            activeStones.Remove(stone);
        }

        stone.SetActive(false);
        pool.Enqueue(stone);
    }
    private void HandleReset(Events.OnGameReset e)
    {
        for (int i = activeStones.Count - 1; i >= 0; i--)
        {
            Return(activeStones[i]);
        }
    }
}