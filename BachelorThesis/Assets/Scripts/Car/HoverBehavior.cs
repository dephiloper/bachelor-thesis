using UnityEngine;

namespace Car
{
    public class HoverBehavior : MonoBehaviour
    {
        [SerializeField] private float _hoverForce = 15f;

        [SerializeField] private float _hoverHeight = 0.5f;

        private Rigidbody _carRigidbody;

        private void Awake()
        {
            _carRigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (_carRigidbody)
                PerformHover();
        }

        private void PerformHover()
        {
            RaycastHit hit;

            if (!Physics.Raycast(transform.position, -transform.up, out hit, _hoverHeight,
                LayerMask.GetMask("Track", "Wall", "Default"))) return;
            
            var proportionalHeight = (_hoverHeight - hit.distance) / _hoverHeight;
            var appliedForce = Vector3.up * proportionalHeight * _hoverForce;
            _carRigidbody.AddForceAtPosition(appliedForce, transform.position, ForceMode.Acceleration);
        }
    }
}