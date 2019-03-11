using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIController : MonoBehaviour, IUseGameEvents {

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

	private void Awake()
	{
		Subscribe();
	}

	private void OnDestroyed()
	{
		Unsubscribe();
	}

	public void Subscribe()
    {
		EventManager.AddListener<Events.KeypadUsedEvent>(KeypadUsed);
        EventManager.AddListener<Events.TerminalUsedEvent>(TerminalUsed);
    }

    public void Unsubscribe()
    {
		EventManager.RemoveListener<Events.KeypadUsedEvent>(KeypadUsed);
        EventManager.RemoveListener<Events.TerminalUsedEvent>(TerminalUsed);
    }
	
	public void KeypadUsed(Events.KeypadUsedEvent eventArgs)
	{
		_screenTint.SetActive(true);
		_keypad.SetActive(true);

		_keypad.GetComponent<KeypadController>().Initialize(eventArgs.Code);

		OnUIOpened();
	}

	public void KeypadClosed()
	{
		_screenTint.SetActive(false);
		_keypad.SetActive(false);

		OnUIClosed();
	}

	public void TerminalUsed(Events.TerminalUsedEvent eventArgs)
	{
		_screenTint.SetActive(true);
		_terminal.SetActive(true);

		_terminal.GetComponent<TerminalController>().Initialize(eventArgs.TerminalContent);

		OnUIOpened();
	}

	public void TerminalClosed()
	{
		_screenTint.SetActive(false);
		_terminal.SetActive(false);

		OnUIClosed();
	}
}
