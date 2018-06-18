using UnityEngine;

namespace FiniteStateMachine.States
{
    public class HitState : BaseState
    {
        public HitState(Transform transform) : base(transform)
        {
            Setup();
        }

        #region Actions

       
        private void Hit()
        {
            /*var ballRb = Enemy.Target.GetComponent<Rigidbody>();
            ballRb.velocity = - ballRb.velocity;*/
            //Rigidbody.MovePosition(Enemy.Target.position);
        }

        #endregion

        #region Conditions
        
        public bool IsBallHittable()
        {
            var dist = Vector3.Distance(Enemy.Target.position, Transform.position);
            if (dist <= Enemy.HitDistance)
            {
                var heading = Enemy.Target.position - Transform.position;
                var dot = Vector3.Dot(heading, Transform.forward);
                return dot > 0;
            }

            return false;
        }
        
        #endregion
        
        private void Setup()
        {
            EntryActions.Add(Hit);
        }
    }
}