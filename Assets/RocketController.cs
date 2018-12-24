using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour {

	//Tweakable values
	[SerializeField] private float _thrustForce = 1000f;
	[SerializeField] private float _rotationSensitivity = 100f;
	[SerializeField] private bool _firstPersonMode = false;

	//Components
	[SerializeField] private AudioSource _thrustAudioSource;
	[SerializeField] private AudioClip _explosionAudioClip;
	[SerializeField] private AudioClip _collisionAudioClip;

	[SerializeField] private ParticleSystem _thrustParticles;

	private Rigidbody _rigidbody;

	//Inputs
	private float _rotationInput;
	private bool _thrustInput;

	//States
	private enum State
	{
		inactive,
		thrusting,
		dying,
		won
	}

	private State _state;

	void Start()
	{
		_state = State.inactive;	
		AssignReferences();
	}

	private void AssignReferences()
	{
		_rigidbody = GetComponent<Rigidbody>();
	}

	void Update()
	{
		if(_state == State.inactive || _state == State.thrusting) 
		{ 
			GetInput();
			ApplyInput();
		}

		ApplySound();
		ApplyParticles();
	}

	private void GetInput()
	{
		if(_firstPersonMode)
		{
			_rotationInput = Input.GetAxisRaw("Vertical");
		}
		else
		{
			_rotationInput = Input.GetAxisRaw("Horizontal");
		}
		
		_thrustInput = Input.GetButton("Jump");
	}

	private void ApplyInput()
	{
		_rigidbody.freezeRotation = true;

		ApplyThrust();
		ApplyRotation();

		_rigidbody.freezeRotation = false;
	}

	private void ApplyThrust()
	{
		if(_thrustInput)
		{
			_state = State.thrusting;
			_rigidbody.AddRelativeForce(Vector3.up * _thrustForce * Time.deltaTime);
		}
		else
		{
			_state = State.inactive;
		}
	}

	private void ApplyRotation()
	{
		transform.Rotate(-Vector3.forward * _rotationSensitivity * _rotationInput * Time.deltaTime);
	}

	private void ApplySound()
	{
		if (_state == State.thrusting)
		{
			SoundManager.instance.PlaySound(_thrustAudioSource);
		}
		else
		{
			SoundManager.instance.StopSound(_thrustAudioSource, true);
		}
	}

	private void ApplyParticles()
	{
		if (_state == State.thrusting)
		{
			StartParticles(_thrustParticles);
		}
		else
		{
			StopParticles(_thrustParticles);
		}
	}

	private void StartParticles(ParticleSystem particleSystem)
	{
		if(!particleSystem.isPlaying)
		{
			particleSystem.Play();
		}
	}

	private void StopParticles(ParticleSystem particleSystem)
	{
		if(particleSystem.isPlaying)
		{
			particleSystem.Stop();
		}
	}

	void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;

		switch(tag)
		{
			case "Safe":
				SafeBumb();
				return;
			case "Finish":
				_state = State.won;
				return;
			default:
				_state = State.dying;
				DestroyShip();
				break;
		}
    }

	private void SafeBumb()
	{
		if(_rigidbody.velocity.magnitude > 1f)
		{
			float volume  = _rigidbody.velocity.magnitude / 2f;
			SoundManager.instance.PlaySound(_collisionAudioClip);
		}
	}

	private void DestroyShip()
	{
		SoundManager.instance.PlaySound(_collisionAudioClip);
		StopParticles(_thrustParticles);

		StartCoroutine(DestroyCoroutine());
	}

	private IEnumerator DestroyCoroutine()
	{
		yield return new WaitForSeconds(1f);

		SoundManager.instance.PlaySound(_explosionAudioClip);
		Destroy(gameObject);
	}
}
