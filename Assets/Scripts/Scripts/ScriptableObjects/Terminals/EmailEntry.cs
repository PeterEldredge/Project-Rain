using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EmailEntry : ScriptableObject {

	[SerializeField] private string _title;
	public string Title 
	{ 
		get { return _title; } 
	}

	[SerializeField] [TextArea(10, 20)] private string _content;
	public string Content
	{
		get { return _content; }
	}
}
