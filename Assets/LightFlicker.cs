using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour {

	[SerializeField] private Light _light;

	[SerializeField] private AnimationCurve _onTimeCurve;
	[SerializeField] private AnimationCurve _offTimeCurve;

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
			if(_on) waitTime = _onTimeCurve.Evaluate(Random.Range(0, _onTimeCurve.length));
			else waitTime = _offTimeCurve.Evaluate(Random.Range(0, _offTimeCurve.length));

			yield return new WaitForSeconds(waitTime);

			_on = !_on;
			_light.enabled = _on;
		}
	}
}
