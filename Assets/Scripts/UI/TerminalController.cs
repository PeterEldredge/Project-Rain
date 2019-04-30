using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TerminalController : MonoBehaviour {

	[SerializeField] private List<GameObject> _navigationalButtons;
	[SerializeField] private List<GameObject> _emailEntries;

	[SerializeField] private Text _emailTitle;
	[SerializeField] private Text _emailContent;

	public UnityEvent onTerminalOpened = new UnityEvent();
	private void OnTerminalOpened()
	{
		onTerminalOpened.Invoke();
	}

	public UnityEvent onTerminalClosed = new UnityEvent();
	private void OnTerminalClosed()
	{
		onTerminalClosed.Invoke();
	}

	private LayoutElement _emailContentLayoutElement;

	private TerminalContent _terminalContent;

	private void Awake()
	{
		_emailContentLayoutElement = _emailContent.gameObject.GetComponent<LayoutElement>();
	}

	public void Initialize(TerminalContent terminalContent)
	{
		OnTerminalOpened();
		Reset();

		_terminalContent = terminalContent;

		InitializeEmails();
	}

	public void Reset()
	{
		foreach(GameObject button in _navigationalButtons)
		{
			button.SetActive(false);
		}

		foreach(GameObject button in _emailEntries)
		{
			button.SetActive(false);
		}

		_emailTitle.text = null;
		_emailContent.text = null;
	}

	public void InitializeEmails()
	{
		List<EmailEntry> emails = _terminalContent.EmailEntries;

		for(int i = 0; i < emails.Count; i++)
		{		
			_emailEntries[i].GetComponentInChildren<Text>().text = emails[i].Title;
			_emailEntries[i].SetActive(true);

			if(i == 0)
			{
				_navigationalButtons[0].SetActive(true);
				
				_emailEntries[0].GetComponent<Button>().onClick.Invoke();
			}
		}

		//Quit Button is always the last element
		_navigationalButtons[_navigationalButtons.Count - 1].SetActive(true);
	}

	public void EmailEntryClicked(int entryIndex)
	{
		EmailEntry currentEntry = _terminalContent.EmailEntries[entryIndex];

		_emailTitle.text = currentEntry.Title;
		_emailContent.text = currentEntry.Content;
		_emailContentLayoutElement.preferredHeight = _emailContent.preferredHeight;
	}
}
