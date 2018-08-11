using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AgentData.Actions;
using Car;
using Environment;
using Extensions;
using UnityEngine;

namespace AgentImpl
{
    public class DecisionMakingAgent : Agent
    {
        public float Acc;
        
        private int _waypointId = 1;
        private readonly Dictionary<int, Waypoint> _idToWaypointDict = new Dictionary<int, Waypoint>();

        private void Start()
        {
            foreach (var section in EnvironmentManager.Instance.Sections) {
                    _idToWaypointDict.Add(section.WaypointIdentifier,
                        section.transform.GetComponentInChildren<Waypoint>());
            }
        }

        protected override void Compute()
        {
            if (!GameManager.Instance.StartRace) return;

            base.Compute();
            var target = FindNextTarget();
            var relativeDir = transform.InverseTransformPoint(target.transform.position);
            var steer = (relativeDir.x / relativeDir.magnitude) * 2f;

            if (steer > 0.05f)
                steer = 1;
            else if (steer < -0.05f)
                steer = -1;
            else
                steer = 0;
            
            
            /*var acc = 1f;

            Debug.DrawRay(transform.position, transform.forward * 3, Color.magenta);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 3f,
                LayerMask.GetMask("Wall")))
            {
                acc = hit.distance.Map(5, 0, 0.8f, 0.2f);
            }

            print(acc);*/

            Acc = target.Acceleration;
            if (SpeedIncreaseTime == 0)
                Acc *= SpeedIncreaseFactor;
            
            Acc = Acc > 1 ? 1f : Acc;
            

            PerformAction(new PlayerAction(steer, Acc, false));
        }

        private Waypoint FindNextTarget()
        {
            var pos = new Vector2(transform.position.x, transform.position.z);
            if (Vector2.Distance(pos, _idToWaypointDict[_waypointId].transform.position.ToVector2()) < 2f)
                _waypointId++;

            if (_waypointId == _idToWaypointDict.Count)
                _waypointId = 1;

            return _idToWaypointDict[_waypointId];
        }
    }
}