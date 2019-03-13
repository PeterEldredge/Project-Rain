using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
	public struct DocumentCollectedEvent : IGameEvent
	{
		public readonly Document Document;

		public DocumentCollectedEvent(Document document)
		{
			Document = document;
		}
	}

	public struct ItemCollectedEvent : IGameEvent
	{
		public readonly Item Item;

		public ItemCollectedEvent(Item item)
		{
			Item = item;
		}
	}

	public struct JournalCollectedEvent : IGameEvent
	{
		public readonly Journal Journal;

		public JournalCollectedEvent(Journal journal)
		{
			Journal = journal;
		}
	}
}

public class Collectable : MonoBehaviour
{
	//MUST BE DONE BECAUSE INTERFACES DO NOT WORK WITH INSPECTOR
	//PS I HATE UNITY
	public enum CollectableType
	{
		Document,
		Item,
		Journal
	}

	public CollectableType collectableType;

	public Document document;
	public Item item;
	public Journal journal;

    [SerializeField] private ICollectable _collectable;

    public void OnUse()
    {
		switch(collectableType)
		{
			case CollectableType.Document:
				_collectable = document;
				break;
			case CollectableType.Item:
				_collectable = item;
				break;
			case CollectableType.Journal:
				_collectable = journal;
				break;
		}

		_collectable.OnUse();

		//I HATE THIS LINE
		Destroy(transform.parent.transform.parent.gameObject);
    }  
}
