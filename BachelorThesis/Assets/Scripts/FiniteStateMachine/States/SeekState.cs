using UnityEngine;

namespace FiniteStateMachine.States
{
    public class SeekState : BaseState
    {
        public SeekState(Transform transform) : base(transform)
        {
            Setup();
        }

        private void Setup()
        {
            Actions.Add(FollowBall);
            Actions.Add(FlipPaddle);
        }

        private void FollowBall()
        {
            var target = Enemy.Target;

            // define boundaries, so that the bat can not move further
            var clampX = Mathf.Clamp(target.position.x, Enemy.StartPosition.x - Enemy.MaxMoveSpace.x,
                Enemy.StartPosition.x + Enemy.MaxMoveSpace.x);
            var clampY = Mathf.Clamp(target.position.y, Enemy.StartPosition.y - Enemy.MaxMoveSpace.y,
                Enemy.StartPosition.y + Enemy.MaxMoveSpace.y);

            // specify the new position of the bat, but follow only on the x and y axis
            var newPosition = new Vector3(clampX, clampY, Transform.position.z);

            // move the bat depending on the specified speed
            var step = Enemy.MoveBatSpeed * Time.deltaTime;
            Rigidbody.MovePosition(Vector3.MoveTowards(Transform.position, newPosition, step));
        }

        internal bool IsBallSeekable()
        {
            var dist = CalulateDistance();
            return dist <= Enemy.SeekDistance;
        }

        private float CalulateDistance()
        {
            return Vector3.Distance(Enemy.Target.position, Transform.position);
        }
    }
}