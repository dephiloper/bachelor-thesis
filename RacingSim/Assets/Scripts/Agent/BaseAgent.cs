using CustomEditors;
using UnityEngine;

namespace Agent
{
    public abstract class BaseAgent
    {
        public Transform Transform { get; }

        protected readonly Rigidbody Rigidbody;
        protected Percept Percept;
        protected bool OnTrack;
        protected readonly AgentEditorProperties EditorProps;
    
        private readonly Sensor _sensor;
        private readonly float _onTrackSpeed;
        private readonly float _offTrackSpeed;

        protected BaseAgent(AgentScript agentScript)
        {
            EditorProps = agentScript.EditorProperties;
            Transform = agentScript.transform;
            Rigidbody = agentScript.GetComponent<Rigidbody>();
            _sensor = new Sensor(Transform);
            
            _onTrackSpeed = EditorProps.MaxSpeed;
            _offTrackSpeed = EditorProps.MaxSpeed / 4f;
        }

        public virtual void Compute()
        {
            OnTrack = IsOnTrack();
            Percept = _sensor.PerceiveEnvironment(OnTrack);
            EditorProps.MaxSpeed = OnTrack ? _onTrackSpeed : _offTrackSpeed;
            EditorProps.Speed = new Vector2(Rigidbody.velocity.x, Rigidbody.velocity.z).magnitude;
        }

        protected void PerformAction(Action action)
        {
            if (action.AccelerateForward)
                Rigidbody.AddForce(Transform.forward * EditorProps.MaxSpeed);
            if (action.AccelerateBackward)
                Rigidbody.AddForce(-Transform.forward * EditorProps.MaxSpeed * 0.1f);
            if (action.SteerLeft)
                Rigidbody.MoveRotation(
                    Quaternion.Euler(Transform.rotation.eulerAngles - Transform.up * EditorProps.TurnSpeed));
            if (action.SteerRight)
                Rigidbody.MoveRotation(
                    Quaternion.Euler(Transform.rotation.eulerAngles + Transform.up * EditorProps.TurnSpeed));
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