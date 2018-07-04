using UnityEngine;

namespace Agent
{
    public abstract class BaseAgent
    {
        public Transform Transform { get; }

        protected readonly Rigidbody Rigidbody;
        protected readonly AgentEditorProperties EditorProps;
        protected Percept Percept;
        protected bool OnTrack;
        protected float Speed;

        private const float BackwardSpeedReduction = 0.2f;
        private readonly Sensor _sensor;
        
        private int _frames;

        protected BaseAgent(AgentScript agentScript)
        {
            EditorProps = agentScript.EditorProperties;
            Transform = agentScript.transform;
            Rigidbody = agentScript.GetComponent<Rigidbody>();
            _sensor = new Sensor(Transform);
        }

        public virtual void Compute()
        {
            Percept = _sensor.PerceiveEnvironment(OnTrack);
            if (_frames % 30 == 0)
                OnTrack = IsOnTrack();

            _frames++;
            Speed = OnTrack ? EditorProps.MaxSpeed : EditorProps.MaxSpeed / 4f;
            Speed *= Rigidbody.drag * 2;
            EditorProps.Speed = Rigidbody.velocity.ToVector2().magnitude;
            EditorProps.Label.text = $"{EditorProps.Speed} km/h";
            EditorProps.TurnSpeed = EditorProps.Speed.Map(0f, EditorProps.MaxSpeed, EditorProps.MaxTurnSpeed, EditorProps.MaxTurnSpeed/2f);
        }

        protected void PerformAction(Action action)
        {
            if (action.AccelerateForward)
            {
                var velocity = SteeringBehaviour.Seek(Transform.position,
                    Transform.position + Transform.forward,
                    Rigidbody.velocity, Speed);
                Rigidbody.AddForce(velocity, ForceMode.Acceleration);
            }
            if (action.AccelerateBackward)
            {
                var velocity = SteeringBehaviour.Seek(Transform.position, 
                    Transform.position - Transform.forward,
                    Rigidbody.velocity, Speed) * BackwardSpeedReduction;
                Rigidbody.AddForce(velocity, ForceMode.Acceleration);
            }
            
            // Steering
            if (action.SteerLeft)
                Rigidbody.MoveRotation(
                    Quaternion.Euler(Transform.rotation.eulerAngles - Transform.up * EditorProps.TurnSpeed * action.SteerValue));
            if (action.SteerRight)
                Rigidbody.MoveRotation(
                    Quaternion.Euler(Transform.rotation.eulerAngles + Transform.up * EditorProps.TurnSpeed * action.SteerValue));
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
    }
}