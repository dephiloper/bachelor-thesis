using Train;
using UnityEngine;

namespace CameraUtils
{
	public class CameraMovementBehaviour : MonoBehaviour {
		private void FixedUpdate ()
		{
			if (TrainManager.Instance.BestAgent == null) return;
		
			transform.position = Vector3.Lerp(transform.position, 
				new Vector3(TrainManager.Instance.BestAgent.transform.position.x, transform.position.y,
					TrainManager.Instance.BestAgent.transform.position.z), Time.deltaTime * 5f);		
		}
	}
}
