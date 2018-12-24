using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour {

	//Tweakable values
	[SerializeField] private float _thrustForce = 1000f;
	[SerializeField] private float _rotationSensitivity = 100f;

	//Components
	private Rigidbody _rigidbody;

	//Inputs
	private float _rotationInput;
	private bool _thrustInput;

	//States
	private enum State
	{
		alive,
		dying,
		won
	}

	private State _state;

	void Start()
	{
		_state = State.alive;	
		AssignReferences();
	}

	private void AssignReferences()
	{
		_rigidbody = GetComponent<Rigidbody>();
	}

	void Update()
	{
		if(_state == State.dying || _state == State.won) { return; }

		GetInput();
		ApplyInput();
	}

	private void GetInput()
	{
		_rotationInput = Input.GetAxisRaw("Horizontal");
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
			_rigidbody.AddRelativeForce(Vector3.up * _thrustForce * Time.deltaTime);
		}
	}

	private void ApplyRotation()
	{
		transform.Rotate(-Vector3.forward * _rotationSensitivity * _rotationInput * Time.deltaTime);
	}

	void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;

		switch(tag)
		{
			case "Safe":
				return;
			case "Finish":
				return;
			default:
				DestroyShip();
				break;
		}
    }

	private void DestroyShip()
	{
		Destroy(gameObject, 1f);
	}
}
