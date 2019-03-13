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

	private string _correctCode;
	private int _currentCodePos;
	private int CurrentCodePos
	{
		get { return _currentCodePos; }
		set
		{
			StopCoroutine("BlinkCurrentCodePos");

			if(_currentCodePos < _code.Length) _code[_currentCodePos].gameObject.SetActive(true);

			_currentCodePos = value;
	
			StartCoroutine("BlinkCurrentCodePos");
		}
	}

	private IEnumerator BlinkCurrentCodePos()
	{
		if(CurrentCodePos != _code.Length)
		{
			GameObject currentText = _code[CurrentCodePos].gameObject;

			while(true)
			{
				yield return new WaitForSeconds(.2f);
				
				currentText.SetActive(!currentText.activeSelf);
			}
		}
	}

	public void Initialize(string code)
	{
		Reset();

		_correctCode = code;
	}

	public void Reset()
	{
		CurrentCodePos = 0;

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
		if(CurrentCodePos == 0)
		{
			//Temporary
			OnCorrectCode();
		}
		else
		{
			CurrentCodePos--;
			_code[CurrentCodePos].text = "-";
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
			//Debug.LogError("Correct code input");
		}
		else
		{
			Reset();
			OnIncorrectCode();
			//Debug.LogError("Incorrect code input");
		}
	}

	private void UpdateCode(string key)
	{
		if(CurrentCodePos < _code.Length)
		{			
			_code[CurrentCodePos].text = key;
			CurrentCodePos++;
		}
	}
}
