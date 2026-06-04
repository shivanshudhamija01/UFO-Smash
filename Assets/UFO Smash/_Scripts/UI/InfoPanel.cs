using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    [SerializeField] private Button exitButton;
    private IEventBus eventBus;
    private IAudioService audioService;
    void Awake()
    {
        eventBus = ServiceLocator.GetService<IEventBus>();
        audioService = ServiceLocator.GetService<IAudioService>();
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }
    void OnExitButtonClicked()
    {
        audioService.UISFX(SoundType.Click);
        eventBus.Publish(new Events.OnCloseButtonClicked());
    }
}
