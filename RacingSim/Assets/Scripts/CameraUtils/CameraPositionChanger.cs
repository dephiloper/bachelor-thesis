using UnityEngine;

namespace CameraUtils
{
	public class CameraPositionChanger : MonoBehaviour {
		private void Update () {
			if (Input.GetKeyDown(KeyCode.Q))
				transform.localPosition = new Vector3(0, 0.6f, -1);
			if (Input.GetKeyDown(KeyCode.E))
				transform.localPosition = new Vector3(0, 0.4f, -0.7f);
			if (Input.GetKeyDown(KeyCode.R))
				transform.localPosition = new Vector3(0, 0.25f, -0.1f);
		}
	}
}
