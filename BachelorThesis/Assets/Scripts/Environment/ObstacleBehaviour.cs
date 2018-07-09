using Agent;
using UnityEngine;

namespace Environment
{
	public class ObstacleBehaviour : MonoBehaviour {
		private void OnCollisionEnter(Collision other)
		{
			var agentScript = other.gameObject.GetComponent<AgentBehaviour>();
			if (!agentScript) return;

			agentScript.Agent.ObstacleCollided();
		}
	}
}
