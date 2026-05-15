using System.Collections.Generic;
using UnityEngine;

public class AnimalPool : MonoBehaviour
{
    public static AnimalPool Instance;

    [Header("Animal Prefabs")]
    public GameObject[] animalPrefabs;

    [Header("Pool Settings")]
    public int poolSizePerAnimal = 10;

    private List<GameObject> pooledAnimals =
        new List<GameObject>();

    private void Awake()
    {
        Instance = this;

        CreatePool();
    }

    void CreatePool()
    {
        for (int i = 0; i < animalPrefabs.Length; i++)
        {
            for (int j = 0; j < poolSizePerAnimal; j++)
            {
                GameObject obj =
                    Instantiate(animalPrefabs[i], transform);

                obj.SetActive(false);

                pooledAnimals.Add(obj);
            }
        }
    }

    public GameObject GetAnimal()
    {
        List<GameObject> inactiveAnimals =
            new List<GameObject>();

        foreach (GameObject obj in pooledAnimals)
        {
            if (!obj.activeInHierarchy)
            {
                inactiveAnimals.Add(obj);
            }
        }

        if (inactiveAnimals.Count == 0)
            return null;

        int randomIndex =
            Random.Range(0, inactiveAnimals.Count);

        return inactiveAnimals[randomIndex];
    }
}