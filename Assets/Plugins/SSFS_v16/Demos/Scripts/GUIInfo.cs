using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIInfo : MonoBehaviour
{
	public string text;

	private void OnGUI()
	{
		GUILayout.Label( new GUIContent( text ) );
	}
}