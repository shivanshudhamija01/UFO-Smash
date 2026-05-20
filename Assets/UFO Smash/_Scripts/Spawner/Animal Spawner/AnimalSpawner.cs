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

        animalObj.transform.position = spawnPoint.position;

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

// Need to add the service to store the currently active animals in the scene and remove on animal removed;

// When the animal is taken, i have to update the service as well as the animal spawner 
// In the animal spawner ,i have to clear the animal count from the lane where the animal is spawned,
// and 