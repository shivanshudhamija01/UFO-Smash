using Unity.Mathematics;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Vector3 position;
    private GameObject player;
    private IEventBus eventBus;
    private void Awake()
    {
        eventBus = ServiceLocator.Get<IEventBus>();
    }

    void OnEnable()
    {
        eventBus.Add<Events.OnGameStarted>(SpawnPlayer);
    }
    void OnDisable()
    {
        eventBus.Remove<Events.OnGameStarted>(SpawnPlayer);
    }
    private void SpawnPlayer(Events.OnGameStarted evt)
    {
        player = Instantiate(playerPrefab, position, quaternion.identity);
    }
}
