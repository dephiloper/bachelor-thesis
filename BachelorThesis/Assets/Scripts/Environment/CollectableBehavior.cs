using Train;
using UnityEngine;

namespace Environment
{
    public class CollectableBehavior : MonoBehaviour
    {
        private const float RotationSpeed = 50;

        private void Update()
        {
            transform.Rotate(Vector3.up * (RotationSpeed * Time.deltaTime));
        }

        private void OnTriggerEnter(Collider other)
        {
            var agent = other.gameObject.GetComponent<AgentImpl.Agent>();
            if (!agent) return;

            agent.CollectableGathered();

            // if training just let the collectable stay, so that all agents could collect them
            if (TrainManager.Instance) return;
            gameObject.SetActive(false);
            Invoke(nameof(Reactivate), 10f);
        }

        private void Reactivate() => gameObject.SetActive(true);
    }
}