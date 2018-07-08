using System.Collections.Generic;
using Agent.Data;
using Car;
using Train;
using UnityEngine;

namespace Agent.AgentImpl
{
    public abstract class BaseAgent
    {
        public Transform Transform { get; }
        public List<Vector3> VisibleCollectables => Percept?.VisibleCollectables;

        protected Percept Percept { get; private set; }
        protected Rigidbody Rigidbody { get; }
        protected AgentEditorProperties EditorProps { get; }
        protected bool OnTrack { get; private set; }
        protected float Speed { get; private set; }

        private const float BackwardSpeedReduction = 0.2f;
        private readonly Sensor _sensor;

        private int _frames;
        private int _speedIncreaseTime;

        protected BaseAgent(AgentBehaviour agentBehaviour)
        {
            EditorProps = agentBehaviour.EditorProperties;
            Transform = agentBehaviour.transform;
            Rigidbody = agentBehaviour.GetComponent<Rigidbody>();
            _sensor = new Sensor(agentBehaviour);
            
        }

        public virtual void Compute()
        {
            UpdateEditorProps();
            
            if (_frames % 5 == 0) {
            Percept = _sensor.PerceiveEnvironment(OnTrack);
            Percept.Normalize(EditorProps.MaxSpeed, EditorProps.SensorDistance, Transform.position,
                EditorProps.ViewRadius);
            }
            if (_frames % 30 == 0)
                OnTrack = IsOnTrack();

            _frames++;
            Speed = OnTrack ? EditorProps.MaxSpeed : EditorProps.MaxSpeed / 4f;
            Speed *= Rigidbody.drag * 2;

            if (_speedIncreaseTime > 0)
            {
                Speed *= 1.25f;
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
                velocity = SteeringBehaviour.Seek(Transform.position,
                    Transform.position + Transform.forward,
                    Rigidbody.velocity, Speed);
            }

            if (action.AccelerateBackward)
            {
                velocity = SteeringBehaviour.Seek(Transform.position,
                               Transform.position - Transform.forward,
                               Rigidbody.velocity, Speed) * BackwardSpeedReduction;
            }

            if (velocity != Vector3.zero)
            {
                Rigidbody.AddForce(velocity * action.AccelerateValue, ForceMode.Acceleration);
            }

            // Steering
            if (action.SteerLeft)
                Rigidbody.MoveRotation(
                    Quaternion.Euler(Transform.rotation.eulerAngles -
                                     Transform.up * EditorProps.TurnSpeed * action.SteerValue));
            if (action.SteerRight)
                Rigidbody.MoveRotation(
                    Quaternion.Euler(Transform.rotation.eulerAngles +
                                     Transform.up * EditorProps.TurnSpeed * action.SteerValue));
        }

        protected virtual void UpdateEditorProps()
        {
            EditorProps.Speed = Rigidbody.velocity.ToVector2().magnitude;
            EditorProps.TurnSpeed =
                EditorProps.Speed.Map(0f, EditorProps.MaxSpeed, EditorProps.MaxTurnSpeed,
                    EditorProps.MaxTurnSpeed / 2f);

            if (TrainManager.Instance)
                EditorProps.ShowSensors = TrainManager.Instance.ShowSensors;
            else 
                EditorProps.Label.text = $"{EditorProps.Speed} km/h";
            
        }

        private bool IsOnTrack()
        {
            if (Physics.Raycast(Transform.position, -Transform.up, float.MaxValue, LayerMask.GetMask("Track")))
            {
                return !Physics.Raycast(Transform.position, -Transform.up, float.MaxValue,
                    LayerMask.GetMask("Wall"));
            }

            return false;
        }

        public virtual void CollectableGathered() => _speedIncreaseTime += 2000;
    }
}