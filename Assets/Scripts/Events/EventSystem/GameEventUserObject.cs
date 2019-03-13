using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameEventUserObject : MonoBehaviour, IUseGameEvents
{
    private void Awake()
    {
        Subscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    public virtual void Subscribe() {}
    public virtual void Unsubscribe() {}
}
