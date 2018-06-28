﻿using UnityEngine;

public class CameraMovement : MonoBehaviour {
	private void FixedUpdate () {
		transform.position = Vector3.Lerp(transform.position, 
			new Vector3(TrainManager.Instance.BestAgent.Transform.position.x, transform.position.y,
				TrainManager.Instance.BestAgent.Transform.position.z), Time.deltaTime * 5f);		
	}
}
