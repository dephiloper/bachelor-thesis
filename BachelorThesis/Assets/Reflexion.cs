using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reflektion : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnTriggerEnter(Collider other)
	{
		print("collision"); //TODO find a way to perform a collision
		other.attachedRigidbody.velocity = -other.attachedRigidbody.velocity;
		other.attachedRigidbody.angularVelocity = -other.attachedRigidbody.angularVelocity;
	}
}
