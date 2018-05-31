using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulateMovementScript : MonoBehaviour
{
	public Vector3 StartPos;
	public float LaunchTime = 2.3f;
	public float LaunchSpeed = 100f;
	
	private Rigidbody _batRb;
	
	void Start () {
		_batRb = GetComponent<Rigidbody>();
		InvokeRepeating(nameof(RelocateBat), 0f, 4.0f); 
		InvokeRepeating(nameof(LaunchBat), LaunchTime, 4.0f);
		InvokeRepeating(nameof(StopBat), 2.5f, 4.0f);
		StartPos = transform.position;
	}
	
	private void RelocateBat()
	{
		transform.position = StartPos;
	}

	private void LaunchBat()
	{
		_batRb.AddForce(Vector3.forward * LaunchSpeed);
	}
	
	private void StopBat()
	{
		_batRb.velocity = Vector3.zero;
	}
}
