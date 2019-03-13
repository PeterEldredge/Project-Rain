using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Collectable/Item")]
public class Item : ScriptableObject, ICollectable
{
    [SerializeField] private string _name;
    public string Name
    {
        get { return _name; }
    }

    [SerializeField] [TextArea(2, 3)] private string _description;
    public string Description
    {
        get { return _description; }
    }

    [SerializeField] private Image _image;
    public Image Image
    {
        get { return _image; }
    }

    [SerializeField] int _index;
    public int Index
    {
        get { return _index; }
    }

    public void OnUse()
    {
        EventManager.TriggerEvent(new Events.ItemCollectedEvent(this));
    }
}
