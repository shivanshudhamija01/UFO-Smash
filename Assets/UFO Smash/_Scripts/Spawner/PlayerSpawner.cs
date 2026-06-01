using Unity.Mathematics;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Vector3 position;
    [SerializeField] private VariableJoystick variableJoystick;
    private GameObject player;
    private IEventBus eventBus;
    private void Awake()
    {
        eventBus = ServiceLocator.Get<IEventBus>();
    }

    void OnEnable()
    {
        eventBus.Add<Events.OnGameStarted>(SpawnPlayer);
        eventBus.Add<Events.OnGameReset>(ResetSpawner);
    }
    void OnDisable()
    {
        eventBus.Remove<Events.OnGameStarted>(SpawnPlayer);
        eventBus.Remove<Events.OnGameReset>(ResetSpawner);
    }
    private void SpawnPlayer(Events.OnGameStarted evt)
    {
        player = Instantiate(playerPrefab, position, quaternion.identity);
        Aim playerAim = player.GetComponent<Aim>();
        playerAim.SetVariableJoystick(variableJoystick);
    }
    private void ResetSpawner(Events.OnGameReset evt)
    {
        player.gameObject.SetActive(false);
        player.transform.position = position;
    }
}
