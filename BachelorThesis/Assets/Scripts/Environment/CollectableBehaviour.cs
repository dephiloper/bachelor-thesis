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
			var agent = other.gameObject.GetComponent<AgentImpl.Agent>();
			if (!agent) return;

			agent.CollectableGathered();
			Destroy(gameObject);
		}
	}
}