using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsScript : MonoBehaviour
{
	public GameObject Controller;	
	private SteamVR_TrackedObject _trackedObj;
	private SteamVR_Controller.Device TrackedDevice => SteamVR_Controller.Input((int) _trackedObj.index);

	
	// Use this for initialization
	void Start ()
	{
		_trackedObj = Controller.GetComponent<SteamVR_TrackedObject>();
		
	}
	
	// Update is called once per frame
	void Update () {
	}

	/*private void OnTriggerEnter(Collider other)
	{
		if (other.name.Equals("Ball"))
		{
			var fixedJoint = other.gameObject.AddComponent<FixedJoint>();
			fixedJoint.connectedBody = gameObject.GetComponent<Rigidbody>();
			Destroy(fixedJoint, 0.2f);
			var velocityScript = Controller.GetComponent<VelocityScript>();
			var ballRb = other.GetComponent<Rigidbody>();
			ballRb.velocity = velocityScript.AverageVelocity;
			ballRb.angularVelocity = velocityScript.AverageAngularVelocity;

		}
	}*/

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.name.Equals("Ball")) {
			TrackedDevice.TriggerHapticPulse(3999);
		}
	}
}
