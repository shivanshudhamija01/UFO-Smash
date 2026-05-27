using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button exitButton;
    private IEventBus eventBus;

    private void Awake()
    {
        eventBus = ServiceLocator.Get<IEventBus>();
        playButton.onClick.AddListener(OnPlayButtonClicked);
        settingButton.onClick.AddListener(OnSettingButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void OnPlayButtonClicked()
    {
        // Fire an event
        eventBus.Publish(new Events.OnGameStarted());
    }
    private void OnSettingButtonClicked()
    {
        // Fire an event ;
    }
    private void OnExitButtonClicked()
    {
        // Fire an event ;
    }
}
