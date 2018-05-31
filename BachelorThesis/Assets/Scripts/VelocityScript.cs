using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class VelocityScript : MonoBehaviour
{
	private const int Capacity = 10;
	
	private Queue<Vector3[]> _velocities;
	private SteamVR_TrackedObject _trackedObj;
	private SteamVR_Controller.Device TrackedDevice => SteamVR_Controller.Input((int) _trackedObj.index);

	public Vector3 AverageVelocity
	{
		get
		{
			var sum = Vector3.zero;
			
			foreach (var velocity in _velocities)
			{
				sum += velocity[0];
			}

			return sum / Capacity;
		}
	}
	
	public Vector3 AverageAngularVelocity
	{
		get
		{
			var sum = Vector3.zero;
			
			foreach (var velocity in _velocities)
			{
				sum += velocity[1];
			}

			return sum / Capacity;
		}
	}

	private void Awake()
	{
		_trackedObj = GetComponent<SteamVR_TrackedObject>();
	}


	void Start () {
		_velocities = new Queue<Vector3[]>(Capacity + 1);
	}
	
	void Update ()
	{
		_velocities.Enqueue(new [] {TrackedDevice.velocity, TrackedDevice.angularVelocity});
		
		if (_velocities.Count > Capacity)
		{
			_velocities.Dequeue();
		} 
	}
}
