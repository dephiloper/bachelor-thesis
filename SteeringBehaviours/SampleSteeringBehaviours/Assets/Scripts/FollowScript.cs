using UnityEngine;

public class FollowScript : MonoBehaviour {
	// Update is called once per frame
	private void Update ()
	{
		var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePosition.z = 0;
		transform.position = mousePosition;
	}
}