using System;
public interface IEventBus
{
    void Add<T>(Action<T> listener) where T : IGameEvent;
    void Remove<T>(Action<T> listener) where T : IGameEvent;
    void Publish<T>(T gameEvent) where T : IGameEvent;
}
