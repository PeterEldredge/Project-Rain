using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public struct GameStartedEventArgs : IGameEvent { }

    public struct GameOverEventArgs : IGameEvent
    {
        private float _time;
        public float Time
        {
            get { return _time; }
        }

        public GameOverEventArgs(float time)
        {
            this._time = time;
        }
    }
}