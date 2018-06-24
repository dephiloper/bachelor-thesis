using System.Linq;
using UnityEngine;

namespace Car
{
	public class Agent : MonoBehaviour
	{
		public Brain Brain;
		
		[SerializeField]
		private bool _isHuman = true;
		
		[SerializeField]
		private float _speed = 5f;

		[SerializeField]
		private float _turnSpeed = 2f;

		private float _onTrackSpeed;
		private float _offTrackSpeed;
		private Rigidbody _rigidbody;
		private DriverSensor _driverSensor;
		
		private void Start ()
		{
			_rigidbody = GetComponent<Rigidbody>();
			_driverSensor = new DriverSensor(transform);
			
			_onTrackSpeed = _speed;
			_offTrackSpeed = _speed / 4f;
		}
	
		private void FixedUpdate ()
		{
			var action = new Action();
			var percept = _driverSensor.PerceiveEnvironment();

			if (_isHuman)
				action = new Action(Input.GetAxisRaw("P1Horizontal"),Input.GetAxisRaw("P1Vertical"));
			else if (Brain != null)
				action = Brain.Think(percept);
			
			PerformActions(action);

			_speed = _offTrackSpeed;
			
			if (IsOnTrack())
			{
				_speed = _onTrackSpeed;
				Brain.Score = percept.WallDistances.Sum();
			}
		}


		private void PerformActions(Action action)
		{
			if (action.AccelerateForward)
				_rigidbody.AddForce(transform.forward * _speed);
			if (action.AccelerateBackward)
				_rigidbody.AddForce(-transform.forward * _speed);
			if (action.SteerLeft)
				_rigidbody.MoveRotation(Quaternion.Euler(transform.rotation.eulerAngles - transform.up * _turnSpeed));
			if (action.SteerRight)
				_rigidbody.MoveRotation(Quaternion.Euler(transform.rotation.eulerAngles + transform.up * _turnSpeed));
		}

		private bool IsOnTrack()
			=> Physics.Raycast(transform.position, -transform.up, float.MaxValue, LayerMask.GetMask("Track"));

	}
}
