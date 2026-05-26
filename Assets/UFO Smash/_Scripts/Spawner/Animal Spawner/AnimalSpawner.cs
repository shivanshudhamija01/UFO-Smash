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
    private IEventBus eventBus;
    private void Awake()
    {
        animalService = ServiceLocator.Get<IAnimalService>();
        eventBus = ServiceLocator.Get<IEventBus>();
    }

    private void Start()
    {
    }
    private void OnEnable()
    {
        eventBus.Add<Events.OnGameStarted>(SpawnAnimals);
    }
    private void OnDisable()
    {
        eventBus.Remove<Events.OnGameStarted>(SpawnAnimals);
    }
    private void SpawnAnimals(Events.OnGameStarted evt)
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

            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);

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

            Lane randomLane = lanes[index];

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

        Transform spawnPoint = moveAToB ? selectedLane.pointA : selectedLane.pointB;

        Transform targetPoint = moveAToB ? selectedLane.pointB : selectedLane.pointA;

        // Get pooled animal
        GameObject animalObj = AnimalPool.Instance.GetAnimal();

        if (animalObj == null)
            return;

        // Spawn position
        Vector3 spawnPos = spawnPoint.position;

        spawnPos.z = 0;

        animalObj.transform.position = spawnPos;

        animalObj.SetActive(true);

        // Sorting Order Logic
        int sortingOrder;

        if (!selectedLane.firstOrderTaken)
        {
            sortingOrder = 2 * index;

            selectedLane.firstOrderTaken = true;
        }
        else
        {
            sortingOrder = (2 * index) + 1;

            selectedLane.secondOrderTaken = true;
        }

        selectedLane.currentAnimals++;

        AnimalController animal = animalObj.GetComponent<AnimalController>();

        animal.Initialize(targetPoint, this, selectedLane, sortingOrder, moveAToB);

        currentAnimals++;

        animalService.AddAnimal(animal);
    }

    public void AnimalRemoved(AnimalController animal)
    {
        currentAnimals = Mathf.Max(0, currentAnimals - 1);

        Lane lane = animal.AssignedLane;

        if (lane != null)
        {
            int laneIndex = System.Array.IndexOf(lanes, lane);

            int firstOrder = 2 * laneIndex;

            int secondOrder = firstOrder + 1;

            int animalOrder = animal.GetSortingOrder();

            if (animalOrder == firstOrder)
            {
                lane.firstOrderTaken = false;
            }
            else if (animalOrder == secondOrder)
            {
                lane.secondOrderTaken = false;
            }

            lane.currentAnimals = Mathf.Max(0, lane.currentAnimals - 1);
        }

        animalService.RemoveAnimal(animal);
    }
}