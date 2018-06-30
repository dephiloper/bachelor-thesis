using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderScript : MonoBehaviour {

	private void Awake()
	{
		//note that Waypoints must be in the right order in the editor
		for (var i = 0; i < transform.childCount; i++)
			transform.GetChild(i).GetComponent<Waypoint>().WaypointIdentifier = i+1;
	}
}
