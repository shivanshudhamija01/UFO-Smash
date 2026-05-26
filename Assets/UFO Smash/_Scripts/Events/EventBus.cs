using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EventBus : IEventBus
{
    private readonly Dictionary<Type, Delegate> events = new();
    public void Add<T>(Action<T> listener) where T : IGameEvent
    {
        var type = typeof(T);
        if (!events.ContainsKey(type)) events[type] = null;
        events[type] = Delegate.Combine(events[type], listener);
    }

    public void Remove<T>(Action<T> listener) where T : IGameEvent
    {
        var type = typeof(T);
        if (events.TryGetValue(type, out var listeners))
        {
            var result = Delegate.Remove(listeners, listener);
            if (result == null)
            {
                events.Remove(type);
            }
            else
            {
                events[type] = result;
            }
        }
    }
    public void Publish<T>(T gameEvent) where T : IGameEvent
    {
        var type = typeof(T);
        if (events.TryGetValue(type, out var listener))
        {
            var callbackmethods = listener as Action<T>;
            callbackmethods?.Invoke(gameEvent);
        }
    }

}
