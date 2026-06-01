using UnityEngine;

public class GameBootStrapper : MonoBehaviour
{
    void Awake()
    {
        RegisterServices();
    }
    void RegisterServices()
    {
        ServiceLocator.Register<IEventBus>(new EventBus());
        ServiceLocator.Register<IAnimalService>(new AnimalService());
        ServiceLocator.Register<IScoreService>(new ScoreService());
    }
}
