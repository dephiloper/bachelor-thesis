using UnityEngine;

namespace CameraUtils
{
	public class CameraStabilizer : MonoBehaviour
	{
		public GameObject Vehicle;
		public float SmoothRotationSpeed = 10f;
	
		private void FixedUpdate ()
		{
			transform.position = Vehicle.transform.position;
		
			// smoothens the rotation by interpolation between current roation and the roation of the vehicle
			var desiredRotation = Vehicle.transform.rotation;
			var smoothedRotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * SmoothRotationSpeed);
		
			// applies only the y rotation to stabilize the camera
			transform.eulerAngles = Vector3.up * smoothedRotation.eulerAngles.y;
		}
	}
}
