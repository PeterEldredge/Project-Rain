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
    [SerializeField] private ICollectable _collectable;

    public void OnUse()
    {
		_collectable.OnUse();
    }  
}
