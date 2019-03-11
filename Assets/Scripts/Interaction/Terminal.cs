using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public struct TerminalUsedEvent : IGameEvent
    {
        public readonly TerminalContent TerminalContent;

        public TerminalUsedEvent(TerminalContent terminalContent)
        {
            TerminalContent = terminalContent;
        }
    }
}

public class Terminal : MonoBehaviour, IUsable
{
    [SerializeField] private TerminalContent _terminalContent;

    public void OnUse()
    {
        EventManager.TriggerEvent(new Events.TerminalUsedEvent(_terminalContent));
    }
}
