using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UsableTriggers : MonoBehaviour {

	public enum ObjectType { Standard, Collectable }
	public enum CollectableType { Document, Item, Journal }

	public ObjectType objectType;
	public CollectableType collectableType;

	public Document document;
	public Item item;
	public Journal journal;

	public UnityEvent onUsed = new UnityEvent();
	public void OnUse()
	{
		onUsed.Invoke();
	}
}
