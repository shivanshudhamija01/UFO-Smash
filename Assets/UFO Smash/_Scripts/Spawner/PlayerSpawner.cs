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
        eventBus = ServiceLocator.GetService<IEventBus>();
    }

    private void OnEnable()
    {
        eventBus.Add<Events.OnGameStarted>(SpawnPlayer);
        eventBus.Add<Events.OnGameReset>(ResetSpawner);
    }

    private void OnDisable()
    {
        eventBus.Remove<Events.OnGameStarted>(SpawnPlayer);
        eventBus.Remove<Events.OnGameReset>(ResetSpawner);
    }

    private void SpawnPlayer(Events.OnGameStarted evt)
    {
        // Create only once
        if (player == null)
        {
            player = Instantiate(
                playerPrefab,
                position,
                quaternion.identity);

            Aim playerAim = player.GetComponent<Aim>();

            playerAim.SetVariableJoystick(variableJoystick);
        }

        player.transform.position = position;

        player.SetActive(true);
    }

    private void ResetSpawner(Events.OnGameReset evt)
    {
        if (player == null)
            return;

        player.transform.position = position;

        player.SetActive(false);
    }
}