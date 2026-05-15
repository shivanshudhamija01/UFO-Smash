using UnityEngine;

public class GameBootStrapper : MonoBehaviour
{
    void Awake()
    {
        RegisterServices();
    }
    void RegisterServices()
    {
        ServiceLocator.Register<IAnimalService>(new AnimalService());
    }
}
