using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Collectable/Document")]
public class Document : ScriptableObject, ICollectable
{
    [SerializeField] private string _name;
    public string Name
    {
        get { return _name; }
    }

    [SerializeField] private string _title;
    public string Title
    {
        get { return _title; }
    }

    [SerializeField] [TextArea(5, 15)] private string _content;
    public string Content
    {
        get { return _content; }
    }

    [SerializeField] int _index;
    public int Index
    {
        get { return _index; }
    }

    public void OnUse()
    {
        EventManager.TriggerEvent(new Events.DocumentCollectedEvent(this));
    }
}
