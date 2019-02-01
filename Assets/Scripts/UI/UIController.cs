using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIController : MonoBehaviour {

	[SerializeField] private GameObject _screenTint;
	[SerializeField] private GameObject _keypad;
	[SerializeField] private GameObject _terminal;

	public UnityEvent onUIOpened = new UnityEvent();
	public void OnUIOpened()
	{
		onUIOpened.Invoke();
	}

	public UnityEvent onUIClosed = new UnityEvent();
	public void OnUIClosed()
	{
		onUIClosed.Invoke();
	}
	
	public void KeypadUsed(string code)
	{
		_screenTint.SetActive(true);
		_keypad.SetActive(true);

		_keypad.GetComponent<KeypadController>().Initialize(code);

		OnUIOpened();
	}

	public void KeypadClosed()
	{
		_screenTint.SetActive(false);
		_keypad.SetActive(false);

		OnUIClosed();
	}

	public void TerminalUsed(TerminalContent terminalContent)
	{
		_screenTint.SetActive(true);
		_terminal.SetActive(true);

		_terminal.GetComponent<TerminalController>().Initialize(terminalContent);

		OnUIOpened();
	}

	public void TerminalClosed()
	{
		_screenTint.SetActive(false);
		_terminal.SetActive(false);

		OnUIClosed();
	}
}
