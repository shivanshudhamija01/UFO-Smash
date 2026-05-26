using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gamePlayPanel;


    private IEventBus eventBus;
    private void Awake()
    {
        eventBus = ServiceLocator.Get<IEventBus>();
    }
    private void OnEnable()
    {
        eventBus.Add<Events.OnGameStarted>(OnGameStart);
    }
    private void OnDisable()
    {
        eventBus.Remove<Events.OnGameStarted>(OnGameStart);
    }


    private void OnGameStart(Events.OnGameStarted evt)
    {
        mainMenuPanel.SetActive(false);
        gamePlayPanel.SetActive(true);
    }
}
