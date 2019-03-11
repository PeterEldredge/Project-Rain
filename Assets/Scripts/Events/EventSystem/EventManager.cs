using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    private delegate void EventListener(IGameEvent gameEventArgs);
    private static Dictionary<Type, List<EventListener>> _eventListeners;

    public static void AddListener<T>(Action<T> listener) where T : IGameEvent
    {
        Type eventType = typeof(T); 

        if(_eventListeners == null)
        {
            _eventListeners = new Dictionary<Type, List<EventListener>>();
        }

        if(!_eventListeners.ContainsKey(eventType))
        {
            _eventListeners.Add(eventType, new List<EventListener>());
        }

        EventListener wrapper = (gameEventArgs) => { listener((T)gameEventArgs); };

        _eventListeners[eventType].Add(wrapper);
    }

    public static void TriggerEvent(IGameEvent gameEventArgs)
    {
        Type eventType = gameEventArgs.GetType(); 

        if(_eventListeners.ContainsKey(eventType))
        {
            for(int i = 0; i < _eventListeners[eventType].Count; i++)
            {
                _eventListeners[eventType][i].Invoke(gameEventArgs);
            }
        }
    }

    public static void RemoveListener<T>(Action<T> listener) where T : IGameEvent
    {
        Type eventType = typeof(T);

        if(_eventListeners != null && _eventListeners.ContainsKey(eventType))
        {
            EventListener wrapper = (gameEventArgs) => { listener((T)gameEventArgs); };
            _eventListeners[eventType].Remove(wrapper);
        }
    }
}
