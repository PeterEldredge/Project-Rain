using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour {

	[SerializeField] private Light _light;

	[SerializeField] private float _minOnTime;
	[SerializeField] private float _maxOnTime;

	[SerializeField] private float _minOffTime;
	[SerializeField] private float _maxOffTime;

	private bool _on;

	private void Awake()
	{
		_on = _light.enabled;

		StartCoroutine(FlickerRoutine());
	}

	private IEnumerator FlickerRoutine()
	{
		while(true)
		{
			float waitTime;
			if(_on) waitTime = Random.Range(_minOnTime, _maxOnTime);
			else waitTime = Random.Range(_minOffTime, _maxOffTime);

			yield return new WaitForSeconds(waitTime);

			_on = !_on;
			_light.enabled = _on;
			//_light.SetLightDirty();
		}
	}
}
