using System.Collections.Generic;
using Car;
using Environment;
using Extensions;
using UnityEngine;

namespace AgentImpl
{
    public class MovementAgent : Agent
    {
        private int _waypointId = 1;
        private readonly Dictionary<int, Vector2> _idToWaypointDict = new Dictionary<int, Vector2>();

        private void Start()
        {
            foreach (var section in EnvironmentManager.Instance.Sections)
                _idToWaypointDict.Add(section.WaypointIdentifier, section.transform.GetChild(0).position.ToVector2());
        }

        protected override void Compute()
        {
            if (!GameManager.Instance.StartRace) return;
            
            base.Compute();
            var target = FindNextTarget();
            Rigidbody.AddForce(SteeringBehavior.Seek(transform.position, target, Rigidbody.velocity,
                Speed), ForceMode.Acceleration);
            AdjustRotation();
        }

        private void AdjustRotation()
        {
            var angle = Mathf.Atan2(Rigidbody.velocity.z, Rigidbody.velocity.x) * Mathf.Rad2Deg + 270f;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.down);
        }

        private Vector3 FindNextTarget()
        {
            var pos = new Vector2(transform.position.x, transform.position.z);
            if (Vector2.Distance(pos, _idToWaypointDict[_waypointId]) < 2f)
                _waypointId++;

            if (_waypointId == _idToWaypointDict.Count)
                _waypointId = 1;

            return _idToWaypointDict[_waypointId].ToVector3();
        }
    }
}