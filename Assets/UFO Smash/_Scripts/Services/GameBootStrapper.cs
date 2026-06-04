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
        ServiceLocator.RegisterService<IEventBus>(eventBus);

        var animalService = new AnimalService();
        ServiceLocator.RegisterService<IAnimalService>(animalService);

        var scoreService = new ScoreService();
        ServiceLocator.RegisterService<IScoreService>(scoreService);

        var audioService = new AudioService(audioManager);
        ServiceLocator.RegisterService<IAudioService>(audioService);
    }
    void SetServiceReference()
    {
        // eventBus = ServiceLocator.Get<IEventBus>();
        // animalService = ServiceLocator.Get<IAnimalService>();
        // scoreService = ServiceLocator.Get<IScoreService>();
        audioService = ServiceLocator.GetService<IAudioService>();
    }
    void Initialize()
    {
        float savedBGM = PlayerPrefs.GetFloat(Keys.BGM, 1f);
        float savedSFX = PlayerPrefs.GetFloat(Keys.SFX, 1f);

        audioService.SetBGMVolume(savedBGM);
        audioService.SetSFXVolume(savedSFX);

        audioService.BGM(SoundType.BGM);
    }

}
