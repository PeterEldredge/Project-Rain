using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public static SoundManager instance;

	[SerializeField] private GameObject _camera;

	//Components
	private AudioSource _managerAudioSource;

	//Lists
	private Dictionary<AudioSource, float> _audioSourcesInRoutine;

	void Awake()
	{
		instance = this;
	}

	void Start()
	{	
		Initializations();
		AssignReferences();
	}

	private void Initializations()
	{
		_audioSourcesInRoutine = new Dictionary<AudioSource, float>();
	}

	private void AssignReferences()
	{
		_managerAudioSource = GetComponent<AudioSource>();
	}

	void Update()
	{
		UpdatePosition();
	}

	private void UpdatePosition()
	{
		_managerAudioSource.transform.position = _camera.transform.position;
	}

	public void PlaySound(AudioSource audioSource)
	{
		if(!audioSource.isPlaying)
		{
			audioSource.Play();
		}
		else if (_audioSourcesInRoutine.ContainsKey(audioSource))
		{
			ResetAudioSource(audioSource);
			audioSource.Play();
		} 
	}

	public void PlaySound(AudioClip audioClip, float volume = 1f)
	{
		_managerAudioSource.PlayOneShot(audioClip, volume);
	}

	private void ResetAudioSource(AudioSource audioSource)
	{
		StopCoroutine(AudioFadeOut(audioSource));
		audioSource.volume = _audioSourcesInRoutine[audioSource];
		_audioSourcesInRoutine.Remove(audioSource);
	}

	public void StopSound(AudioSource audioSource, bool fade)
	{
		if(audioSource.isPlaying)
		{
			if(!fade)
			{
				audioSource.Stop();
			}
			else
			{
				if(_audioSourcesInRoutine.ContainsKey(audioSource))
				{
					ResetAudioSource(audioSource);
				}

				StartCoroutine(AudioFadeOut(audioSource));
			}
		}
	}

	private IEnumerator AudioFadeOut(AudioSource audioSource)
	{
		float startVolume = audioSource.volume;
		float time = .2f;

		_audioSourcesInRoutine.Add(audioSource, startVolume);

		while (audioSource.volume > .01f)
		{
			audioSource.volume = Mathf.Lerp(0f, startVolume, time);
			time -= Time.deltaTime;

			yield return null;
		}

		_audioSourcesInRoutine.Remove(audioSource);

		audioSource.Stop();
		audioSource.volume = startVolume;
	}
}
