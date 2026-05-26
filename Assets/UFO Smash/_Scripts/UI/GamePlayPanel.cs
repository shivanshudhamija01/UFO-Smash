using System;
using System.Collections.Generic;
using TMPro;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class GamePlayPanel : MonoBehaviour
{
    [SerializeField] private Button pauseButton;
    [SerializeField] private TextMeshProUGUI scoreTxt;
    [SerializeField] private TextMeshProUGUI waveCountTxt;
    [SerializeField] private List<Image> animalAlive;
    private IEventBus eventBus;
    private IScoreService scoreService;
    private int index = 0;
    void Awake()
    {
        eventBus = ServiceLocator.Get<IEventBus>();
        scoreService = ServiceLocator.Get<IScoreService>();
        pauseButton.onClick.AddListener(OnGamePause);
    }
    private void OnEnable()
    {
        eventBus.Add<Events.OnAnimalTaken>(UpdateAnimalAlive);
        eventBus.Add<Events.OnUFODestroyed>(UpdateScore);
    }
    private void OnDisable()
    {
        eventBus.Remove<Events.OnAnimalTaken>(UpdateAnimalAlive);
        eventBus.Remove<Events.OnUFODestroyed>(UpdateScore);
    }
    void OnGamePause()
    {
        Debug.Log("Game Pause");
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
}
