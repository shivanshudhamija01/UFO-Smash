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

    public float minMoveSpeed = 1f;
    public float maxMoveSpeed = 3f;

    private int currentAnimals;

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

        animalObj.transform.position = spawnPoint.position;

        animalObj.SetActive(true);

        // Random speed
        float speed =
            Random.Range(minMoveSpeed, maxMoveSpeed);

        Animal animal =
            animalObj.GetComponent<Animal>();

        animal.Initialize(
            targetPoint,
            speed,
            this,
            selectedLane, index + 1);

        currentAnimals++;
        selectedLane.currentAnimals++;
    }

    public void AnimalRemoved(Animal animal)
    {
        currentAnimals--;
    }
}