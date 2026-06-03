using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gamePlayPanel;
    [SerializeField] private GameObject gamePausePanel;
    [SerializeField] private GameObject gameLostPanel;
    [SerializeField] private GameObject settingPanel;

    private IEventBus eventBus;
    private void Awake()
    {
        mainMenuPanel.SetActive(true);
        eventBus = ServiceLocator.Get<IEventBus>();
    }
    private void OnEnable()
    {
        eventBus.Add<Events.OnGameStarted>(OnGameStart);
        eventBus.Add<Events.OnGamePaused>(OnGamePaused);
        eventBus.Add<Events.OnGameResumed>(OnGameResumed);
        eventBus.Add<Events.OnGameOver>(OnGameOver);
        eventBus.Add<Events.OnReturnToHome>(ReturnToHome);
        eventBus.Add<Events.OnGameRestarted>(OnGameRestart);
        eventBus.Add<Events.OnSettingButtonClicked>(OpenUpSettingPanel);
        eventBus.Add<Events.OnCloseButtonClicked>(CloseSettingPanel);
    }
    private void OnDisable()
    {
        eventBus.Remove<Events.OnGameStarted>(OnGameStart);
        eventBus.Remove<Events.OnGamePaused>(OnGamePaused);
        eventBus.Remove<Events.OnGameResumed>(OnGameResumed);
        eventBus.Remove<Events.OnGameOver>(OnGameOver);
        eventBus.Remove<Events.OnReturnToHome>(ReturnToHome);
        eventBus.Remove<Events.OnGameRestarted>(OnGameRestart);
        eventBus.Remove<Events.OnSettingButtonClicked>(OpenUpSettingPanel);
        eventBus.Remove<Events.OnCloseButtonClicked>(CloseSettingPanel);
    }


    private void OnGameStart(Events.OnGameStarted evt)
    {
        mainMenuPanel.SetActive(false);
        gamePlayPanel.SetActive(true);
    }
    private void OnGamePaused(Events.OnGamePaused evt)
    {
        gamePlayPanel.SetActive(false);
        gamePausePanel.SetActive(true);
    }
    private void OnGameResumed(Events.OnGameResumed evt)
    {
        gamePausePanel.SetActive(false);
        gamePlayPanel.SetActive(true);
    }
    private void OnGameOver(Events.OnGameOver evt)
    {
        gamePausePanel.SetActive(false);
        gamePlayPanel.SetActive(false);
        gameLostPanel.SetActive(true);
    }
    private void ReturnToHome(Events.OnReturnToHome evt)
    {
        gamePlayPanel.SetActive(false);
        gamePausePanel.SetActive(false);
        gameLostPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
    private void OnGameRestart(Events.OnGameRestarted evt)
    {
        gameLostPanel.SetActive(false);
        gamePausePanel.SetActive(false);
        gamePlayPanel.SetActive(true);
    }
    private void OpenUpSettingPanel(Events.OnSettingButtonClicked evt)
    {
        mainMenuPanel.SetActive(false);
        settingPanel.SetActive(true);
    }
    private void CloseSettingPanel(Events.OnCloseButtonClicked evt)
    {
        mainMenuPanel.SetActive(true);
        settingPanel.SetActive(false);
    }
}
