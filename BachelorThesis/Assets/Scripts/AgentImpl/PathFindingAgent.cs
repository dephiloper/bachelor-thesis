using System.Collections.Generic;
using Car;
using Environment;
using Extensions;
using UnityEngine;

namespace AgentImpl
{
    public class PathFindingAgent : Agent
    {
        private int _waypointId = 1;
        private readonly Dictionary<int, Vector2> _idToWaypointDict = new Dictionary<int, Vector2>();

        private void Start()
        {
            foreach (var waypoint in EnvironmentManager.Instance.Waypoints)
                _idToWaypointDict.Add(waypoint.WaypointIdentifier, waypoint.transform.position.ToVector2());
        }

        protected override void Compute()
        {
            if (!GameManager.Instance.StartRace) return;
            
            base.Compute();
            var target = FindNextTarget();
            Rigidbody.AddForce(SteeringBehaviour.Seek(transform.position, target, Rigidbody.velocity,
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