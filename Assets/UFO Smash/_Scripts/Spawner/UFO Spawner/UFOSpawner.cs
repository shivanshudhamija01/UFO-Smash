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
    private int currentWave = 1;
    private int aliveUFOCount = 0;
    private bool isWaveRunning;
    void Start()
    {
        StartCoroutine(WaveRoutine());
    }
    private IEnumerator WaveRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(SpawnWave());
            yield return new WaitUntil(() => aliveUFOCount <= 0);
            yield return new WaitForSeconds(waveDelay);
            currentWave++;
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

            SpawnUFO(profile);
            waveBudget -= profile.Cost;
            Debug.Log("Wave Budget cost is : " + waveBudget);
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
            Debug.LogWarning($"No pooled UFO available for {profile.UfoType}");
            return;
        }

        // Activate UFO
        ufo.SetActive(true);

        // Reset transform if needed
        ufo.transform.position = Vector3.zero;

        // Initialize controller
        UFOController controller = ufo.GetComponent<UFOController>();
        if (controller != null)
        {
            SplineContainer spline = GetRandomSpline();
            controller.Initialize(spline);
        }

        aliveUFOCount++;
    }
    private void HandleUFOFinished(UFO ufo)
    {
        // Here i will writing the code for to set the ufo back to pool
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

                    validProfiles
                        .Add(profile);

                    break;
            }
        }

        if (validProfiles.Count == 0)
            return null;

        return validProfiles[
            Random.Range(
                0,
                validProfiles.Count)];
    }
    private SplineContainer GetRandomSpline()
    {
        if (availableSplines == null || availableSplines.Count == 0)
        {
            Debug.LogWarning("No spline assigned!");
            return null;
        }

        int index = Random.Range(0, availableSplines.Count);

        return availableSplines[index];
    }
    // Se how this gonna work is that , i have a wave and teaching system 
    // so the second wave will be spawned after a certain interval of time or before the interval if all the ufo are destoryed 
    // or after a delay if the player is unable to destroy the ufo's

    // what i needed in this UFOSpawner is that , 

    // i have a wave coroutine and 
}
