using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightIntensityChanger : MonoBehaviour {

	[SerializeField] private Light _light;
	
	[SerializeField] private float _minIntensity;
	[SerializeField] private float _maxIntensity;

	[SerializeField] private float _changeSpeed;

	private void Awake()
	{
		StartCoroutine(IntensityChanger());
	}

	private IEnumerator IntensityChanger()
	{
		float intensityDifference = _maxIntensity - _minIntensity;
		while(true)
		{
			float perlinNoise = Mathf.PerlinNoise(Time.time * _changeSpeed, 0);
			
			_light.intensity = _minIntensity + perlinNoise * intensityDifference;

			yield return 0f;
		}
	}
}
