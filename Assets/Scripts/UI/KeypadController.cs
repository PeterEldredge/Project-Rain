using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class KeypadController : MonoBehaviour {

	[SerializeField] Text[] _code;

	public UnityEvent onCorrectCode = new UnityEvent();
	private void OnCorrectCode()
	{
		onCorrectCode.Invoke();
	}

	public UnityEvent onIncorrectCode = new UnityEvent();
	private void OnIncorrectCode()
	{
		onIncorrectCode.Invoke();
	}

	private int _currentCodePos;
	private string _correctCode;

	public void Initialize(string code)
	{
		_correctCode = code;
		Reset();
	}

	public void Reset()
	{
		_currentCodePos = 0;

		for(int i = 0; i < _code.Length; i++)
		{
			_code[i].text = "-";
		}
	}

	public void Clicked(string key)
	{
		switch(key)
		{
			case "Clear":
				ClearCode();
				break;
			case "Enter":
				EnterCode();
				break;
			default:
				UpdateCode(key);
				break;
		}
	}

	private void ClearCode()
	{
		if(_currentCodePos == 0)
		{
			//Temporary
			OnCorrectCode();
		}
		else
		{
			_currentCodePos--;
			_code[_currentCodePos].text = "-";
		}
	}

	private void EnterCode()
	{
		string code = "";

		foreach(Text tempText in _code)
		{
			code += tempText.text;
		}

		if (code == _correctCode)
		{
			OnCorrectCode();
			Debug.LogError("Correct code input");
		}
		else
		{
			Reset();
			OnIncorrectCode();
			Debug.LogError("Incorrect code input");
		}
	}

	private void UpdateCode(string key)
	{
		if(_currentCodePos < _code.Length)
		{			
			_code[_currentCodePos].text = key;
			_currentCodePos++;
		}
	}
}
