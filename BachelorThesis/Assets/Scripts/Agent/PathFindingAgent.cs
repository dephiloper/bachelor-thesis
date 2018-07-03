using System.Collections.Generic;
using UnityEngine;

namespace Agent
{
    public class PathFindingAgent : BaseAgent
    {
        private int _waypointId = 1;
        private readonly Dictionary<int, Vector2> _idToWaypointDict = new Dictionary<int, Vector2>();

        public PathFindingAgent(AgentScript agentScript) : base(agentScript)
        {
            foreach (var waypoint in EditorProps.WaypointsPrefab.GetComponentsInChildren<Waypoint>())
                _idToWaypointDict.Add(waypoint.WaypointIdentifier, waypoint.transform.position.ToVector2());
        }

        public override void Compute()
        {
            base.Compute();
            var target = FindNextTarget();
            Rigidbody.AddForce(SteeringBehaviour.Seek(Transform.position, target, Rigidbody.velocity,
                EditorProps.MaxSpeed));
            AdjustRotation();
        }

        private void AdjustRotation()
        {
            var angle = Mathf.Atan2(Rigidbody.velocity.z, Rigidbody.velocity.x) * Mathf.Rad2Deg + 270f;
            Transform.rotation = Quaternion.AngleAxis(angle, Vector3.down);
        }

        private Vector3 FindNextTarget()
        {
            var pos = new Vector2(Transform.position.x, Transform.position.z);
            if (Vector2.Distance(pos, _idToWaypointDict[_waypointId]) < 2f)
                _waypointId++;

            if (_waypointId == _idToWaypointDict.Count)
                _waypointId = 1;

            return _idToWaypointDict[_waypointId].ToVector3();
        }
    }
}