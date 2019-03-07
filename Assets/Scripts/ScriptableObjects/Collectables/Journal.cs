using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Collectable/Journal")]
public class Journal : ScriptableObject {

    [SerializeField] private string _name;
    public string Name
    {
        get { return _name; }
    }

    [SerializeField] [TextArea(5, 15)] private string _content;
    public string Content
    {
        get { return _content; }
    }

}
