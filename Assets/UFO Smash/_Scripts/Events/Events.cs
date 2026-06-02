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
    public struct OnStoneShot : IGameEvent
    {
        public int CurrentAmmo;
        public OnStoneShot(int stoneLeft)
        {
            CurrentAmmo = stoneLeft;
        }
    }
    public struct OnStoneReloaded : IGameEvent
    {
        public int MaxAmmo;
        public OnStoneReloaded(int maxStones)
        {
            MaxAmmo = maxStones;
        }
    }
    public struct OnGameStarted : IGameEvent { }
    public struct OnGameRestarted : IGameEvent { }
    public struct OnGamePaused : IGameEvent { }
    public struct OnGameResumed : IGameEvent { }
    public struct OnGameReset : IGameEvent { }
    public struct OnReturnToHome : IGameEvent { }
    public struct OnGameOver : IGameEvent { }
    public struct OnSettingButtonClicked : IGameEvent { }
    public struct OnCloseButtonClicked : IGameEvent { }
    public struct OnWaveIncrement : IGameEvent
    {
        public int CurrentWave;
        public OnWaveIncrement(int currentWave)
        {
            CurrentWave = currentWave;
        }
    }
    public struct OnGameInput : IGameEvent
    {
        public int Direction;

        public OnGameInput(int direction)
        {
            Direction = direction;
        }
    }
}
