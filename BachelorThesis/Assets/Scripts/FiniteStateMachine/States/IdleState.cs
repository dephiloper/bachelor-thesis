using UnityEngine;

namespace FiniteStateMachine.States
{
    public class IdleState : BaseState
    {
        private const float MoveBatSpeed = 1f;
        private const float FloatingSpeed = 12f;
        private const float FloatingInterval = 0.01f;

        public IdleState(Transform transform) : base(transform)
        {
            Setup();
        }

        private void IdleMove()
        {
            var yPos = Enemy.StartPosition.y + Mathf.Sin(Time.time * FloatingSpeed) * FloatingInterval;
            var step = MoveBatSpeed * Time.deltaTime;
            var newPosition = new Vector3(Enemy.StartPosition.x, yPos, Enemy.StartPosition.z);
            Rigidbody.MovePosition(Vector3.MoveTowards(Transform.position, newPosition, step));
        }
        
        private void Setup()
        {
            Actions.Add(IdleMove);
            Actions.Add(FlipPaddle);
        }
    }
}