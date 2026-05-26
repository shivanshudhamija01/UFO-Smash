using UnityEngine;

public class Events : MonoBehaviour
{
    // Here i need to fire a two event 
    // One is to modify the score 
    // Other is to modify the animal lost count 
    public struct OnAnimalTaken : IGameEvent
    {

    }
    public struct OnUFODestroyed : IGameEvent
    {

    }
    public struct OnGameStarted : IGameEvent { }
}
