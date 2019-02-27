using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FramerateTracker : MonoBehaviour {

	[SerializeField] private Text _text;

	private float _updateRate;

	private void Awake()
	{
		_updateRate = .1f;

		StartCoroutine(FramerateUpdater());
		StartCoroutine(InputChecker());
	}

	private IEnumerator FramerateUpdater()
	{
		while(true)
		{
			_text.text = (Mathf.Round(1f / Time.deltaTime)).ToString();

			yield return new WaitForSeconds(_updateRate);
		}
	}

	private IEnumerator InputChecker()
	{
		while(true)
		{
			if(Input.GetKeyDown(KeyCode.P)) _text.gameObject.SetActive(!_text.gameObject.activeSelf);

			yield return 0;
		}
	}
}
