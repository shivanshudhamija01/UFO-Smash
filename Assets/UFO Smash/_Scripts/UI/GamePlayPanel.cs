using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayPanel : MonoBehaviour
{
    [SerializeField] private Button pauseButton;
    [SerializeField] private TextMeshProUGUI scoreTxt;
    [SerializeField] private TextMeshProUGUI waveCountTxt;
    [SerializeField] private TextMeshProUGUI stoneCountTxt;
    [SerializeField] private List<Image> animalAlive;
    private IEventBus eventBus;
    private IScoreService scoreService;
    private IAudioService audioService;
    private int index = 0;
    void Awake()
    {
        eventBus = ServiceLocator.Get<IEventBus>();
        scoreService = ServiceLocator.Get<IScoreService>();
        audioService = ServiceLocator.Get<IAudioService>();
        pauseButton.onClick.AddListener(OnGamePause);
        eventBus.Add<Events.OnGameReset>(ResetGame);
    }
    private void OnEnable()
    {
        eventBus.Add<Events.OnAnimalTaken>(UpdateAnimalAlive);
        eventBus.Add<Events.OnUFODestroyed>(UpdateScore);
        eventBus.Add<Events.OnStoneShot>(UpdateStoneCount);
        eventBus.Add<Events.OnStoneReloaded>(UpdateStoneCountToMax);
        eventBus.Add<Events.OnWaveIncrement>(UpdateCurrentWave);
    }
    private void OnDisable()
    {
        eventBus.Remove<Events.OnAnimalTaken>(UpdateAnimalAlive);
        eventBus.Remove<Events.OnUFODestroyed>(UpdateScore);
        eventBus.Remove<Events.OnStoneShot>(UpdateStoneCount);
        eventBus.Remove<Events.OnStoneReloaded>(UpdateStoneCountToMax);
        eventBus.Remove<Events.OnWaveIncrement>(UpdateCurrentWave);
    }
    private void OnDestroy()
    {
        eventBus.Remove<Events.OnGameReset>(ResetGame);
    }
    void OnGamePause()
    {
        Time.timeScale = 0;
        audioService.SFX(SoundType.Click);
        eventBus.Publish(new Events.OnGamePaused());
    }
    private void UpdateScore(Events.OnUFODestroyed evt)
    {
        int score = scoreService.GetScore();
        scoreTxt.text = score.ToString();
    }
    private void UpdateAnimalAlive(Events.OnAnimalTaken evt)
    {
        if (index < animalAlive.Count)
        {
            animalAlive[index].color = new Color(0.5f, 0.5f, 0.5f);
            index++;
        }
    }
    private void UpdateStoneCount(Events.OnStoneShot evt)
    {
        int stoneCount = evt.CurrentAmmo;
        stoneCountTxt.text = stoneCount.ToString();
    }
    private void UpdateStoneCountToMax(Events.OnStoneReloaded evt)
    {
        int stoneCount = evt.MaxAmmo;
        stoneCountTxt.text = stoneCount.ToString();
    }
    private void UpdateCurrentWave(Events.OnWaveIncrement evt)
    {
        int waveNumber = evt.CurrentWave;
        waveCountTxt.text = waveNumber.ToString();
    }
    private void ResetGame(Events.OnGameReset evt)
    {
        index = 0;
        scoreTxt.text = scoreService.GetScore().ToString();
        waveCountTxt.text = "1";
        stoneCountTxt.text = "5";
        Debug.Log("Reset game is called inside the gameplay panel");
        for (int i = 0; i < animalAlive.Count; i++)
        {
            animalAlive[i].color = new Color(1f, 1f, 1f);
        }
    }
}
