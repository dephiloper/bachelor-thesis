using System;
using System.Collections.Generic;
using System.Linq;
using AgentData;
using AgentData.Base;
using AgentData.Percepts;
using AgentData.Sensors;
using Car;
using Environment;
using Extensions;
using Train;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace AgentImpl
{
    [RequireComponent(typeof(AgentSelector))]
    public abstract class Agent : MonoBehaviour
    {
        public const float SpeedIncreaseFactor = 1.25f;

        [Header(nameof(Agent))] public Text Label;
        public float MaxSpeed = 5f;
        public float MaxTurnSpeed = 2f;
        public int Score;
        public float Speed;
        public float TurnSpeed;
        public bool RightDirection = true;

        // Change the SensorType here
        public Sensor Sensor = new DistanceOnlySensor();

        public List<Vector3> VisibleAgents => (Percept as FieldOfViewPercept)?.VisibleAgents;
        public List<Vector3> VisibleCollectables => (Percept as FieldOfViewPercept)?.VisibleCollectables;
        public List<Vector3> VisibleObstacles => (Percept as FieldOfViewPercept)?.VisibleObstacles;
        public bool OnTrack { get; private set; }
        public int ReachedWaypointId;
        public float DistToNextWaypoint;
        public int CurrentLap;
        public int Place;

        protected IPercept Percept { get; private set; }
        protected Rigidbody Rigidbody;
        protected int SpeedIncreaseTime { get; private set; }

        private const float BackwardSpeedReduction = 0.2f;

        private int _frames;
        
        private void Awake()
        {
            Sensor.Setup(this);
            Rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            Compute();
        }

        protected virtual void Compute()
        {
            UpdateEditorProps();

            if (_frames % 5 == 0) 
            {
                Percept = Sensor.PerceiveEnvironment();
                Percept.Normalize();
            }
            if (_frames % 30 == 0)
                OnTrack = IsOnTrack();

            RightDirection = DetermineDirection();

            _frames++;
            Speed = OnTrack ? MaxSpeed : MaxSpeed / 4f;
            Speed *= Rigidbody.drag * 2;

            if (SpeedIncreaseTime > 0)
            {
                Speed *= SpeedIncreaseFactor;
                SpeedIncreaseTime -= (int) (Time.fixedDeltaTime * 1000);
            }
            else
                SpeedIncreaseTime = 0;
        }

        private bool DetermineDirection()
        {
            var nextWaypoint =
                EnvironmentManager.Instance.Sections.FirstOrDefault(x =>
                    x.WaypointIdentifier == ReachedWaypointId + 1);

            if (!nextWaypoint) throw new NullReferenceException("There should always be a next waypoint.");
            
            var dir = nextWaypoint.transform.position.ToVector2() - transform.position.ToVector2();
            DistToNextWaypoint = dir.magnitude;
            if (dir.magnitude > 10) return false;

            
            var cosOfAngle = Vector2.Dot(dir, transform.forward.ToVector2());
            
            return cosOfAngle >= -0.3;
        }

        protected void PerformAction(IAction action)
        {
            var velocity = Vector3.zero;

            if (action.AccelerateForward)
            {
                velocity = SteeringBehavior.Seek(transform.position,
                    transform.position + transform.forward,
                    Rigidbody.velocity, Speed);
            }

            if (action.AccelerateBackward)
            {
                velocity = SteeringBehavior.Seek(transform.position,
                               transform.position - transform.forward,
                               Rigidbody.velocity, Speed) * BackwardSpeedReduction;
            }

            if (velocity != Vector3.zero)
            {
                Rigidbody.AddForce(velocity * action.AccelerateValue, ForceMode.Acceleration);
            }

            // Steering
            if (action.SteerLeft)
                Rigidbody.MoveRotation(
                    Quaternion.Euler(transform.rotation.eulerAngles -
                                     transform.up * TurnSpeed * action.SteerValue));
            if (action.SteerRight)
                Rigidbody.MoveRotation(
                    Quaternion.Euler(transform.rotation.eulerAngles +
                                     transform.up * TurnSpeed * action.SteerValue));
        }

        protected virtual void UpdateEditorProps()
        {
            Speed = Rigidbody.velocity.ToVector2().magnitude;
            TurnSpeed =
                Speed.Map(0f, MaxSpeed, MaxTurnSpeed,
                    MaxTurnSpeed / 2f);

            if (TrainManager.Instance)
                    Sensor.Show = true;
            else if (Label)
                Label.text = $"{GetType().Name}\n{Speed:0.00} km/h\nLap {CurrentLap}\nPlace {Place}";
        }

        private bool IsOnTrack()
        {
            if (Physics.Raycast(transform.position, -transform.up, float.MaxValue, LayerMask.GetMask("Track")))
            {
                return !Physics.Raycast(transform.position, -transform.up, float.MaxValue,
                    LayerMask.GetMask("Wall"));
            }

            return false;
        }

        public virtual void CollectableGathered()
        {
            if (TrainManager.Instance && UnityEngine.Random.value < 0.5f)
                SpeedIncreaseTime += 2000;
        }    

        public virtual void ObstacleCollided()
        {
        }

        public virtual void SetValues(Agent agent)
        {
            Label = agent.Label;
            MaxSpeed = agent.MaxSpeed;
            MaxTurnSpeed = agent.MaxTurnSpeed;
            Score = agent.Score;
            Sensor = agent.Sensor;
            Speed = agent.Speed;
            TurnSpeed = agent.TurnSpeed;
        }

        public virtual void WaypointCrossed(int waypointIdentifier, int lastWaypointIdentifier)
        {
            // when the reached waypoint is the next e.g. 0 = maxId (no waypoints), waypointIdentifier = 1
            // 0 + 1 == 1 -> true
            // skipping a waypoint will not work
            // this way you can skip some waypoints but not just drive multiple times threw the goal
            if (ReachedWaypointId < waypointIdentifier && waypointIdentifier - ReachedWaypointId < lastWaypointIdentifier / 2)
                ReachedWaypointId = waypointIdentifier;

            // all waypoints reached
            if (ReachedWaypointId == lastWaypointIdentifier)
            {
                CurrentLap++;
                ReachedWaypointId = 0;
            }
        }
    }
}