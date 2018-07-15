using System.Collections.Generic;
using AgentData;
using Car;
using Extensions;
using Train;
using UnityEngine;
using UnityEngine.UI;
using Action = AgentData.Action;

namespace AgentImpl
{
    [RequireComponent(typeof(AgentSelector))]
    public abstract class Agent : MonoBehaviour
    {
        [Header(nameof(Agent))] public Text Label;
        public float MaxSpeed = 5f;
        public float MaxTurnSpeed = 2f;
        public int Score;
        public float Speed;
        public float TurnSpeed;
        public Sensor Sensor;

        public List<Vector3> VisibleAgents => Percept?.VisibleAgents;
        public List<Vector3> VisibleCollectables => Percept?.VisibleCollectables;
        public List<Vector3> VisibleObstacles => Percept?.VisibleObstacles;

        protected Percept Percept { get; private set; }
        protected bool OnTrack { get; private set; }
        protected Rigidbody Rigidbody;

        private const float BackwardSpeedReduction = 0.2f;
        private const float SpeedIncreaseFactor = 1.25f;

        private int _frames;
        private int _speedIncreaseTime;

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

            if (_frames % 5 == 0 || !TrainManager.Instance) {
            Percept = Sensor.PerceiveEnvironment(OnTrack);
            Percept.Normalize(MaxSpeed * SpeedIncreaseFactor, Sensor.Range, transform,
                Sensor.ViewRadius);
            }
            if (_frames % 30 == 0 || !TrainManager.Instance)
                OnTrack = IsOnTrack();

            _frames++;
            Speed = OnTrack ? MaxSpeed : MaxSpeed / 4f;
            Speed *= Rigidbody.drag * 2;

            if (_speedIncreaseTime > 0)
            {
                Speed *= SpeedIncreaseFactor;
                _speedIncreaseTime -= (int) (Time.fixedDeltaTime * 1000);
            }
            else
                _speedIncreaseTime = 0;
        }

        protected void PerformAction(Action action)
        {
            var velocity = Vector3.zero;

            if (action.AccelerateForward)
            {
                velocity = SteeringBehaviour.Seek(transform.position,
                    transform.position + transform.forward,
                    Rigidbody.velocity, Speed);
            }

            if (action.AccelerateBackward)
            {
                velocity = SteeringBehaviour.Seek(transform.position,
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
                Sensor.Show = TrainManager.Instance.ShowSensors;
            else
                Label.text = $"{Speed} km/h";
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

        public virtual void CollectableGathered() => _speedIncreaseTime += 2000;

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
    }
}