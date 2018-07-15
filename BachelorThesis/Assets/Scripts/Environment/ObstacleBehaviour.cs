using UnityEngine;

namespace Environment
{
	public class ObstacleBehaviour : MonoBehaviour {
		private void OnCollisionEnter(Collision other)
		{
			var agent = other.gameObject.GetComponent<AgentImpl.Agent>();
			if (!agent) return;

			agent.ObstacleCollided();
		}
	}
}
