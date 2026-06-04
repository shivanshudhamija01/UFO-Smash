using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using NUnit.Framework;
using UnityEngine.Splines;


public class UFOSpawner : MonoBehaviour
{
    [SerializeField] private List<UFOSpawnProfile> ufoProfiles;
    [SerializeField] private AnimationCurve waveCost;
    [SerializeField] private AnimationCurve spawnDelayCurve;
    [SerializeField] private float waveDelay;
    [Header("Spline Paths")]
    [SerializeField] private List<SplineContainer> availableSplines;
    [SerializeField] private SplineContainer bossSpline;
    [SerializeField] private int waveNumber = 1;
    private int currentWave;
    private int aliveUFOCount = 0;
    private bool isWaveRunning;
    private IAnimalService animalService;
    private int occupiedAnimals = 0;
    private IEventBus eventBus;
    private IAudioService audioService;
    private Coroutine waveRoutine;
    private void Awake()
    {
        currentWave = waveNumber;
        animalService = ServiceLocator.GetService<IAnimalService>();
        eventBus = ServiceLocator.GetService<IEventBus>();
        audioService = ServiceLocator.GetService<IAudioService>();
    }
    private void OnEnable()
    {
        UFOController.OnUFOFinished += HandleUFOFinished;
        eventBus.Add<Events.OnGameStarted>(SpawnUFOs);
        eventBus.Add<Events.OnGameReset>(ResetSpawner);
    }
    private void OnDisable()
    {
        UFOController.OnUFOFinished -= HandleUFOFinished;
        eventBus.Remove<Events.OnGameStarted>(SpawnUFOs);
        eventBus.Remove<Events.OnGameReset>(ResetSpawner);
    }
    void SpawnUFOs(Events.OnGameStarted evt)
    {
        if (waveRoutine != null)
            StopCoroutine(waveRoutine);

        eventBus.Publish(new Events.OnWaveIncrement(currentWave));
        waveRoutine = StartCoroutine(WaveRoutine());
    }
    private IEnumerator WaveRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(SpawnWave());
            yield return new WaitUntil(() => aliveUFOCount <= 0);
            yield return new WaitForSeconds(waveDelay);
            currentWave++;
            // Here need to fire an event to notify the ui that okay update the 
            eventBus.Publish(new Events.OnWaveIncrement(currentWave));
            audioService.SFX(SoundType.WaveComplete);
        }
    }
    private IEnumerator SpawnWave()
    {
        isWaveRunning = true;
        TeachingPhase phase = GetCurrentPhase(currentWave);
        int waveBudget = Mathf.RoundToInt(waveCost.Evaluate(currentWave));
        while (waveBudget > 0)
        {
            UFOSpawnProfile profile = GetRandomUFOForPhase(phase, waveBudget);
            if (profile == null)
            {
                break;
            }

            if (CanSpawnUFO(profile))
            {
                SpawnUFO(profile);

                occupiedAnimals += profile.RequiredAnimals;

                waveBudget -= profile.Cost;


            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            float delay = spawnDelayCurve.Evaluate(currentWave);
            yield return new WaitForSeconds(delay);
        }
        isWaveRunning = false;
        yield return null;
    }
    private void SpawnUFO(UFOSpawnProfile profile)
    {
        GameObject ufo = UFOPool.instance.GetUFO(profile.UfoType);

        if (ufo == null)
        {
            return;
        }

        // Activate UFO
        ufo.SetActive(true);

        ufo.transform.position = Vector3.zero;

        // Initialize controller
        UFOController controller = ufo.GetComponent<UFOController>();
        if (controller != null)
        {
            // Here i will check if the UFO to be spawned is boss or not , if it is boss , then i will assign the special boss spline to it 
            SplineContainer spline = GetRandomSpline();
            if (profile.UfoType == UFOType.Boss)
            {
                spline = bossSpline;
            }
            controller.Initialize(spline);
        }

        aliveUFOCount++;
    }
    private void HandleUFOFinished(UFOController ufo)
    {
        aliveUFOCount--;

        UFOSpawnProfile profile = ufoProfiles.Find(p => p.UfoType == ufo.GetUFOType());

        if (profile != null)
        {
            occupiedAnimals -= profile.RequiredAnimals;

            occupiedAnimals = Mathf.Max(0, occupiedAnimals);
        }

        UFOPool.instance.SetBackToPool(ufo.gameObject, ufo.GetUFOType());
    }
    private TeachingPhase GetCurrentPhase(int wave)
    {
        if (wave <= 1) return TeachingPhase.Introduce;

        else if (wave <= 3) return TeachingPhase.Confidence;

        else if (wave <= 5) return TeachingPhase.Mix;

        else if (wave <= 8) return TeachingPhase.Panic;

        else if (wave <= 12) return TeachingPhase.Mastery;

        return TeachingPhase.Survival;
    }
    UFOSpawnProfile GetRandomUFOForPhase(TeachingPhase phase, int remainingBudget)
    {
        List<UFOSpawnProfile> validProfiles = new List<UFOSpawnProfile>();

        foreach (UFOSpawnProfile profile in ufoProfiles)
        {
            if (profile.Cost > remainingBudget)
                continue;

            switch (phase)
            {
                case TeachingPhase.Introduce:

                    if (profile.UfoType == UFOType.Basic)
                    {
                        validProfiles.Add(profile);
                    }
                    break;

                case TeachingPhase.Confidence:

                    if (profile.UfoType == UFOType.Basic || profile.UfoType == UFOType.Fast)
                    {
                        validProfiles.Add(profile);
                    }
                    break;

                case TeachingPhase.Mix:
                    if (profile.UfoType != UFOType.Boss)
                    {
                        validProfiles.Add(profile);
                    }
                    break;

                case TeachingPhase.Panic:

                case TeachingPhase.Mastery:

                case TeachingPhase.Survival:

                    validProfiles.Add(profile);
                    break;
            }
        }

        if (validProfiles.Count == 0)
            return null;

        return validProfiles[Random.Range(0, validProfiles.Count)];
    }
    #region  HELPER-METHODS
    // Will Give a random spline from the list of spline containers
    private SplineContainer GetRandomSpline()
    {
        if (availableSplines == null || availableSplines.Count == 0)
        {
            return null;
        }
        int index = Random.Range(0, availableSplines.Count);

        return availableSplines[index];
    }
    // This method will return whether we can spawn that ufo profile or not 
    private bool CanSpawnUFO(UFOSpawnProfile profile)
    {
        int animalsInScene = animalService.AnimalCountInScene();

        int freeAnimals = animalsInScene - occupiedAnimals;

        // Boss rule:// must be alone
        if (aliveUFOCount == 0 && profile.UfoType == UFOType.Boss)
        {
            return freeAnimals >= profile.RequiredAnimals;
        }

        return freeAnimals >= profile.RequiredAnimals;
    }
    #endregion

    private void ResetSpawner(Events.OnGameReset evt)
    {
        StopAllCoroutines();

        // Return all active UFOs back to pool

        UFOPool.instance.ReturnAll();

        currentWave = waveNumber;
        aliveUFOCount = 0;
        occupiedAnimals = 0;
        isWaveRunning = false;
    }

}

// 1. First it should feel like that okay the difficulty is increasing 
// 2. Secondly make the boss spawn also 
// private bool CanSpawnUFO(UFOSpawnProfile profile)
// {
//     int animalsInScene = animalService.AnimalCountInScene();
//     int freeAnimals = animalsInScene - occupiedAnimals;

//     bool bossAlive =
//         aliveUFOCount > 0 &&
//         GameObject.FindObjectsOfType<UFOController>()
//             .Any(x => x.gameObject.activeSelf &&
//                      x.GetUFOType() == UFOType.Boss);

//     if (bossAlive)
//         return false;

//     if (profile.UfoType == UFOType.Boss)
//         return aliveUFOCount == 0 &&
//                freeAnimals >= profile.RequiredAnimals;

//     return freeAnimals >= profile.RequiredAnimals;
// }