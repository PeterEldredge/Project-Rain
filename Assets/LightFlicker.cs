using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour {

	[SerializeField] private Light _light;

	[SerializeField] private AnimationCurve _onTimeCurve;
	[SerializeField] private AnimationCurve _offTimeCurve;
	[SerializeField] private AnimationCurve _offIntensity;

	private bool _on;
	private float _startingIntensity;

	private void Awake()
	{
		_on = _light.enabled;
		_startingIntensity = _light.intensity;

		StartCoroutine(FlickerRoutine());
	}

	private IEnumerator FlickerRoutine()
	{
		while(true)
		{
			float waitTime;
			if(_on) waitTime = _onTimeCurve.Evaluate(Random.Range(0, _onTimeCurve.length));
			else waitTime = _offTimeCurve.Evaluate(Random.Range(0, _offTimeCurve.length));

			yield return new WaitForSeconds(waitTime);

			_on = !_on;

			if(_offIntensity == null)
			{
				_light.enabled = _on;
			}
			else if(_light.intensity == _startingIntensity)
			{
				_light.intensity = _offIntensity.Evaluate(Random.Range(0, _offIntensity.length));
			}
			else
			{
				_light.intensity = _startingIntensity;
			}
		}
	}
}
