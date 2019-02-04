using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectable : ScriptableObject {

	[SerializeField] private string _name;
    public string Name
    {
        get { return _name; }
    }

}

[CreateAssetMenu]
public class Document : Collectable {

    [SerializeField] private string _title;
    public string Title
    {
        get { return _title; }
    }

    [SerializeField] [TextArea] private string _content;
    public string Content
    {
        get { return _content; }
    }

    [SerializeField] int _index;
    public int Index
    {
        get { return _index; }
    }

}

[CreateAssetMenu]
public class Item : Collectable {

    [SerializeField] private string _description;
    public string Description
    {
        get { return _description; }
    }

    [SerializeField] int _index;
    public int Index
    {
        get { return _index; }
    }

}

[CreateAssetMenu]
public class JournalEntry : Collectable {

    [SerializeField] [TextArea] private string _content;
    public string Content
    {
        get { return _content; }
    }

}