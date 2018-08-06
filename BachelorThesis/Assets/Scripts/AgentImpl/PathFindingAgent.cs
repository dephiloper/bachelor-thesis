using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using AgentData.Actions;
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
            base.Compute();
            var target = FindNextTarget();
            var relativeDir = transform.InverseTransformPoint(target);
            var steer = (relativeDir.x / relativeDir.magnitude);

            RaycastHit hit;
            var acc = 1f;

            if (OnTrack) {
                Debug.DrawRay(transform.position, transform.forward * 5, Color.magenta);
                if (Physics.Raycast(transform.position, transform.forward, out hit, 5f,
                    LayerMask.GetMask("Wall", "Obstacle")))
                {
                    if (LayerMask.GetMask("Obstacle") ==
                        (LayerMask.GetMask("Obstacle") | (1 << hit.collider.gameObject.layer)))
                        steer -= 0.2f;
                    else
                        acc = hit.distance.Map(5, 0, 0.6f, -0.5f);
                }
            }


        PerformAction(new PlayerAction(steer, acc, false));


/*            Rigidbody.AddForce(transform.forward * Speed/2, ForceMode.Acceleration);

            
            Rigidbody.MoveRotation(Quaternion.Euler(transform.rotation.eulerAngles + transform.up * TurnSpeed * steer));*/
        }

        public override void SetValues(Agent agent)
        {
            base.SetValues(agent);
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