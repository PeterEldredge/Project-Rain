using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Collectable/Item")]
public class Item : ScriptableObject {

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

    [SerializeField] int _index;
    public int Index
    {
        get { return _index; }
    }

}
