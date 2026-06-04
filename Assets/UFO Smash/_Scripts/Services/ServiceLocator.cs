using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static Dictionary<Type, object> services = new Dictionary<Type, object>();
    public static void RegisterService<T>(T service)
    {
        services[typeof(T)] = service;
    }
    public static T GetService<T>()
    {
        return (T)services[typeof(T)];
    }
}
