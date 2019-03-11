using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public struct KeypadUsedEvent : IGameEvent
    {
        public readonly string Code;

        public KeypadUsedEvent(string code)
        {
            Code = code;
        }
    }
}

public class Keypad : MonoBehaviour, IUsable
{
    [SerializeField] private string _code;

    public void OnUse()
    {
        EventManager.TriggerEvent(new Events.KeypadUsedEvent(_code));
    }
}
