using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UsableTriggers : MonoBehaviour {

	public UnityEvent onUsed = new UnityEvent();
	public void OnUse()
	{
		onUsed.Invoke();
	}
}
