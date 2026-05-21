using System.Collections;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    [Header("Lanes")]
    public Lane[] lanes;

    [Header("Spawn Settings")]
    public int maxAnimals = 6;

    public int maxAnimalsPerLane = 2;

    public float minSpawnDelay = 1f;
    public float maxSpawnDelay = 3f;
    private int currentAnimals;
    private IAnimalService animalService;
    private void Awake()
    {
        animalService = ServiceLocator.Get<IAnimalService>();
    }
    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (currentAnimals < maxAnimals)
            {
                SpawnAnimal();
            }

            float delay =
                Random.Range(minSpawnDelay, maxSpawnDelay);

            yield return new WaitForSeconds(delay);
        }
    }

    void SpawnAnimal()
    {
        Lane selectedLane = null;

        int safety = 20;
        int index = 0;
        while (safety > 0)
        {
            index = Random.Range(0, lanes.Length);
            Lane randomLane =
                lanes[index];

            if (randomLane.currentAnimals < maxAnimalsPerLane)
            {
                selectedLane = randomLane;
                break;
            }

            safety--;
        }

        if (selectedLane == null)
            return;

        // Debug.Log("Animal is spawned in the lane : " + index);
        // Direction
        bool moveAToB = Random.value > 0.5f;

        Transform spawnPoint =
            moveAToB ? selectedLane.pointA : selectedLane.pointB;

        Transform targetPoint =
            moveAToB ? selectedLane.pointB : selectedLane.pointA;

        // Get pooled animal
        GameObject animalObj =
            AnimalPool.Instance.GetAnimal();

        if (animalObj == null)
            return;

        Vector3 spawnPos = spawnPoint.position;
        spawnPos.z = 0;
        animalObj.transform.position = spawnPos;

        animalObj.SetActive(true);

        AnimalController animal =
            animalObj.GetComponent<AnimalController>();

        animal.Initialize(
            targetPoint,
            this,
            selectedLane, index + 1, moveAToB);

        currentAnimals++;
        animalService.AddAnimal(animal);
        selectedLane.currentAnimals++;
    }

    public void AnimalRemoved(AnimalController animal)
    {
        currentAnimals = Mathf.Max(0, currentAnimals - 1);
        animalService.RemoveAnimal(animal);
    }
}