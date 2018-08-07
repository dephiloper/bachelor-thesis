using UnityEngine;

namespace Environment
{
	public class OrderScript : MonoBehaviour {

		private void Awake()
		{
			//note that Waypoints must be in the right order in the editor
			for (var i = 0; i < transform.childCount; i++)
				transform.GetChild(i).GetComponent<SectionBehavior>().WaypointIdentifier = i+1;
		}
	}
}