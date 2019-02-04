using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollectableList : MonoBehaviour {

	private Document[] _documents;
    public Document[] Documents
    {
        get { return _documents; }
    }

    private Item[] _items;
    public Item[] Items
    {
        get { return _items; }
    }

    private List<JournalEntry> _journal;
    public List<JournalEntry> Journal
    {
        get { return _journal; }
    }

    private void Awake()
    {
        _documents = new Document[20];
        _items = new Item[20];
        _journal = new List<JournalEntry>();
    }
}
