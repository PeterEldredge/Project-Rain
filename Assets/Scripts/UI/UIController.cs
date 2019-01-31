using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIController : MonoBehaviour {

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

	[SerializeField] private GameObject _screenTint;
	[SerializeField] private GameObject _keypad;
	
	public void KeypadUsed(string code)
	{
		_screenTint.SetActive(true);
		_keypad.SetActive(true);

		_keypad.GetComponent<KeypadController>().SetCode(code);

		OnUIOpened();
	}

	public void KeypadClosed()
	{
		_screenTint.SetActive(false);
		_keypad.SetActive(false);

		OnUIClosed();
	}
}
