using UnityEngine;

public class GameBootStrapper : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    private IEventBus eventBus;
    private IAnimalService animalService;
    private IScoreService scoreService;
    private IAudioService audioService;
    void Awake()
    {
        RegisterServices();
        SetServiceReference();
        Initialize();
    }
    void RegisterServices()
    {
        // ServiceLocator.Register<IEventBus>(new EventBus());
        // ServiceLocator.Register<IAnimalService>(new AnimalService());
        // ServiceLocator.Register<IScoreService>(new ScoreService());
        // ServiceLocator.Register<IAudioService>(new AudioService(audioManager));
        var eventBus = new EventBus();
        ServiceLocator.Register<IEventBus>(eventBus);

        var animalService = new AnimalService();
        ServiceLocator.Register<IAnimalService>(animalService);

        var scoreService = new ScoreService();
        ServiceLocator.Register<IScoreService>(scoreService);

        var audioService = new AudioService(audioManager);
        ServiceLocator.Register<IAudioService>(audioService);
    }
    void SetServiceReference()
    {
        // eventBus = ServiceLocator.Get<IEventBus>();
        // animalService = ServiceLocator.Get<IAnimalService>();
        // scoreService = ServiceLocator.Get<IScoreService>();
        audioService = ServiceLocator.Get<IAudioService>();
    }
    void Initialize()
    {
        float savedBGM = PlayerPrefs.GetFloat("BGM", 1f);
        float savedSFX = PlayerPrefs.GetFloat("SFX", 1f);

        audioService.SetBGMVolume(savedBGM);
        audioService.SetSFXVolume(savedSFX);

        audioService.BGM(SoundType.BGM);
    }

}
