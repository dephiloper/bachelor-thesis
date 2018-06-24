using System.Collections.Generic;
using UnityEngine;

public class DriverSensor
{
	
	private const float RayDistance = 10f;
	private const float NoCollisionDistance = RayDistance * 2f;
	
	private readonly LayerMask _wallMask;
	private readonly LayerMask _carMask;
	private readonly Rigidbody _rigidbody;
	private readonly Transform _transform;

	public DriverSensor(Transform transform)
	{
		_transform = transform;
		_rigidbody = transform.GetComponent<Rigidbody>();
		_wallMask = LayerMask.GetMask("Wall");
		_carMask = LayerMask.GetMask("Car");
	}
	
	/// <summary>
	/// Collects information about the environment of the car (sorroundings like distances to walls,
	/// distances to other cars, current velocity).
	/// </summary>
	/// <returns>
	/// Returns collected distance and velocity information.
	/// distance -45 degree (0), 0 degree (1), 45 degree (2)(distance Wall)
	/// distance -45 degree (3), 0 degree (4), 45 degree (5)(distance Enemy)
	/// current velocity x (6) and y (7)
	/// </returns>
	public Percept PerceiveEnvironment ()
	{
		var percept = new Percept
		{
			WallDistances = new List<double>
			{
				ShootRay((_transform.forward - _transform.right).normalized, _wallMask),
				ShootRay(_transform.forward.normalized, _wallMask),
				ShootRay((_transform.forward + _transform.right).normalized, _wallMask)
			},
			PlayerDistances = new List<double>
			{
				ShootRay((_transform.forward - _transform.right).normalized, _carMask),
				ShootRay(_transform.forward.normalized, _carMask),
				ShootRay((_transform.forward + _transform.right).normalized, _carMask)
			},
			//TODO: see if the 0 at y is a good idea
			Velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z)
		};
		
		DrawSensors(percept);
		return percept;
	}
	
	private double ShootRay(Vector3 direction, LayerMask layerMask)
	{
		RaycastHit hit;
		
		if (!Physics.Raycast(_transform.position, direction, out hit, RayDistance, layerMask)) 
			return NoCollisionDistance; 	// normally max distance is RayDistance, so times 2 means like free to go there
		
		return Vector3.Distance(hit.point, _transform.position);
	}
	
	private void DrawSensors(Percept percept)
	{
		if (percept == null) return;
        
		Debug.DrawRay(_transform.position, (_transform.forward - _transform.right).normalized * RayDistance,
			percept.WallDistances[0] < NoCollisionDistance ? Color.red : Color.green);
		Debug.DrawRay(_transform.position, _transform.forward.normalized * RayDistance, 
			percept.WallDistances[1] < NoCollisionDistance ? Color.red : Color.green);
		Debug.DrawRay(_transform.position, (_transform.forward + _transform.right).normalized * RayDistance,
			percept.WallDistances[2] < NoCollisionDistance ? Color.red : Color.green);
	}
}