using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TerminalContent : ScriptableObject {

	[SerializeField] private List<EmailEntry> _emailEntries;
	public List<EmailEntry> EmailEntries
	{
		get { return _emailEntries; }
	}
}
