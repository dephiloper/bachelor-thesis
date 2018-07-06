using Agent;
using UnityEngine;

namespace Environment
{
	public class CollectableBehaviour : MonoBehaviour
	{
		private const float RotationSpeed = 50;

		// Update is called once per frame
		void Update ()
		{
			transform.Rotate(Vector3.up * (RotationSpeed * Time.deltaTime));
		}

		private void OnTriggerEnter(Collider other)
		{
			var agentScript = other.gameObject.GetComponent<AgentBehaviour>();
			if (!agentScript) return;

			agentScript.Agent.CollectableGathered();
			Destroy(gameObject);
		}
	}
}