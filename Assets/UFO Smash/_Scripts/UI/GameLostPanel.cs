using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;
public class GameLostPanel : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button homeButton;
    private IEventBus eventBus;
    void Awake()
    {
        eventBus = ServiceLocator.Get<IEventBus>();
        restartButton.onClick.AddListener(OnGameReset);
        homeButton.onClick.AddListener(OnHomeButtonClicked);
    }

    void OnGameReset()
    {
        eventBus.Publish(new Events.OnGameReset());
        StartCoroutine(RestartGame());

    }
    void OnHomeButtonClicked()
    {
        Time.timeScale = 1;
        eventBus.Publish(new Events.OnGameReset());
        eventBus.Publish(new Events.OnReturnToHome());
    }
    private IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(0.5f);
        eventBus.Publish(new Events.OnGameRestarted());
        eventBus.Publish(new Events.OnGameStarted());
    }
}

// On Game Restart , i have to add a little delay , to invoke the game play event 
