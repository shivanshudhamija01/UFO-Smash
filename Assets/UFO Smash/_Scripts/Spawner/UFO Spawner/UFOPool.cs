using System.Collections.Generic;
using UnityEngine;

public class UFOPool : MonoBehaviour
{
    public static UFOPool instance;

    [SerializeField] private List<UFOSpawnProfile> ufoSO;

    [SerializeField] private int basicUFOCount;
    [SerializeField] private int fastUFOCount;
    [SerializeField] private int shieldUFOCount;
    [SerializeField] private int bossUFOCount;

    [SerializeField] private Transform basicUFOSpawnParent;
    [SerializeField] private Transform fastUFOSpawnParent;
    [SerializeField] private Transform shieldUFOSpawnParent;
    [SerializeField] private Transform bossUFOSpawnParent;

    private GameObject basicUFO;
    private GameObject fastUFO;
    private GameObject shieldUFO;
    private GameObject bossUFO;

    private Queue<GameObject> basicUFOPool =
        new Queue<GameObject>();

    private Queue<GameObject> fastUFOPool =
        new Queue<GameObject>();

    private Queue<GameObject> shieldUFOPool =
        new Queue<GameObject>();

    private Queue<GameObject> bossUFOPool =
        new Queue<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        for (int i = 0; i < ufoSO.Count; i++)
        {
            if (ufoSO[i].UfoType == UFOType.Basic)
            {
                basicUFO = ufoSO[i].UfoPrefab;
            }
            else if (ufoSO[i].UfoType == UFOType.Fast)
            {
                fastUFO = ufoSO[i].UfoPrefab;
            }
            else if (ufoSO[i].UfoType == UFOType.Shield)
            {
                shieldUFO = ufoSO[i].UfoPrefab;
            }
            else if (ufoSO[i].UfoType == UFOType.Boss)
            {
                bossUFO = ufoSO[i].UfoPrefab;
            }
        }
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        InitializePool(basicUFO, basicUFOCount, basicUFOPool, basicUFOSpawnParent);
        InitializePool(fastUFO, fastUFOCount, fastUFOPool, fastUFOSpawnParent);
        InitializePool(shieldUFO, shieldUFOCount, shieldUFOPool, shieldUFOSpawnParent);
        InitializePool(bossUFO, bossUFOCount, bossUFOPool, bossUFOSpawnParent);
    }

    private void InitializePool(GameObject obj, int count, Queue<GameObject> pool, Transform parent)
    {
        if (obj == null)
        {
            Debug.LogWarning("UFO prefab is missing.");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            GameObject temp = Instantiate(obj, parent);
            temp.SetActive(false);
            pool.Enqueue(temp);
        }
    }

    public GameObject GetUFO(UFOType ufoType)
    {
        Queue<GameObject> selectedPool = GetSelectedPool(ufoType);

        if (selectedPool == null || selectedPool.Count == 0)
        {
            return null;
        }
        return selectedPool.Dequeue();
    }

    public void SetBackToPool(GameObject obj, UFOType type)
    {
        Queue<GameObject> selectedPool = GetSelectedPool(type);

        if (selectedPool == null)
            return;

        obj.SetActive(false);
        selectedPool.Enqueue(obj);
    }

    private Queue<GameObject> GetSelectedPool(UFOType type)
    {
        switch (type)
        {
            case UFOType.Basic:
                return basicUFOPool;

            case UFOType.Fast:
                return fastUFOPool;

            case UFOType.Shield:
                return shieldUFOPool;

            case UFOType.Boss:
                return bossUFOPool;
        }
        return null;
    }
}