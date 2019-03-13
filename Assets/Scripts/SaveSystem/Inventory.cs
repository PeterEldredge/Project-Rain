using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;

public class Inventory : MonoBehaviour, ISaveable, IUseGameEvents
{
	//UNSURE OF THIS
	public string SAVE_FILE
    {
        get
        {
            return SaveSystem.SAVE_FOLDER + "/inventory.txt";
        }
    }

	[SerializeField] private Document[] _documents;
	public Document[] Documents
	{
		get { return _documents; }
	}

	[SerializeField] private Item[] _items;
	public Item[] Items
	{
		get { return _items; }
	}

	[SerializeField] private List<Journal> _journals;
	public List<Journal> Journals
	{
		get { return _journals; }
	}

    public Inventory()
	{
		//NUMBERS MUST BE UPDATED WHEN FINAL NUM OF DDOCUMENTS IS DECIDED
		_documents = new Document[5];
		_items = new Item[5];
		_journals = new List<Journal>();
	}

	public Inventory(Document[] documents, Item[] items, List<Journal> journals)
	{
		_documents = documents;
		_items = items;
		_journals = journals;
	}

	#region EVENT_HANDLING
	private void Awake()
	{
		Subscribe();
	}

	private void OnDestroy()
	{
		Unsubscribe();
	}

	public void Subscribe()
    {
        EventManager.AddListener<Events.DocumentCollectedEvent>(DocumentCollected);
		EventManager.AddListener<Events.ItemCollectedEvent>(ItemCollected);
		EventManager.AddListener<Events.JournalCollectedEvent>(JournalCollected);
    }

    public void Unsubscribe()
    {
        EventManager.RemoveListener<Events.DocumentCollectedEvent>(DocumentCollected);
		EventManager.RemoveListener<Events.ItemCollectedEvent>(ItemCollected);
		EventManager.RemoveListener<Events.JournalCollectedEvent>(JournalCollected);
    }

	private void DocumentCollected(Events.DocumentCollectedEvent eventArgs)
	{
		_documents[eventArgs.Document.Index] = eventArgs.Document;
	}

    private void ItemCollected(ItemCollectedEvent eventArgs)
    {
		_items[eventArgs.Item.Index] = eventArgs.Item;
    }

	private void JournalCollected(JournalCollectedEvent eventArgs)
    {
		_journals.Add(eventArgs.Journal);
    }
	#endregion

	#region JSON
	[ContextMenu("SAVE")]
	public void Save()
	{
		string json = JsonUtility.ToJson(new JsonFriendlyInventory(this));
		SaveSystem.Save(SAVE_FILE, json);
	}

	[ContextMenu("LOAD")]
	public void Load()
	{
		string json = SaveSystem.Load(SAVE_FILE);
		JsonFriendlyInventory jsonInventory = JsonUtility.FromJson<JsonFriendlyInventory>(json);

		_documents = jsonInventory.Documents;
		_items = jsonInventory.Items;
		_journals = jsonInventory.ArrayToList(jsonInventory.Journals);
	}

    [System.Serializable]
	private class JsonFriendlyInventory
	{
		public Document[] Documents;
		public Item[] Items;
		public Journal[] Journals;

		public JsonFriendlyInventory(Inventory inventory)
		{
			Documents = inventory.Documents;
			Items = inventory.Items;
			Journals = ListToArray(inventory.Journals);
		}

		public T[] ListToArray<T>(List<T> list)
		{
			T[] array = new T[list.Count];

			for(int i = 0; i < list.Count; i++)
			{
				array[i] = list[i];
			}

			return array;
		}

		public List<T> ArrayToList<T>(T[] array)
		{
			List<T> list = new List<T>();

			for(int i = 0; i < array.Length; i++)
			{
				list.Add(array[i]);
			}
   
			return list;
		}
	}
	#endregion
}  