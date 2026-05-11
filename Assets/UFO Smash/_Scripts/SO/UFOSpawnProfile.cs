using UnityEngine;
[CreateAssetMenu(fileName = "UFO", menuName = "SO/UFO")]
public class UFOSpawnProfile : ScriptableObject
{
    [Header("Type")]
    public UFOType UfoType;
    [Header("Prefab")]
    public GameObject UfoPrefab;
    [Header("Spawn Cost")]
    [Range(1, 20)]
    public int Cost;
    [Header("Spawning Probability")]
    [Range(0, 1)]
    public float SpawnWeight;
}
