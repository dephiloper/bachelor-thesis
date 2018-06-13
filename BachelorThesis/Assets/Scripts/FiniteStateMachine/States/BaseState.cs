using System;
using System.Collections.Generic;
using FiniteStateMachine.Interfaces;
using UnityEngine;

namespace FiniteStateMachine.States
{
    public class BaseState
    {
        public List<Action> Actions { get; private set; }
        public List<Action> EntryActions { get; private set; }
        public List<Action> ExitActions { get; private set; }
        public List<ITransition> Transitions { get; private set; }

        protected readonly Transform Transform;
        protected readonly Rigidbody Rigidbody;
        protected readonly EnemyManager Enemy;

        protected BaseState(Transform transform)
        {
            Transform = transform;
            Rigidbody = transform.GetComponent<Rigidbody>();
            Enemy = transform.GetComponent<EnemyManager>();
            
            Setup();
        }

        protected void FlipPaddle()
        {
            // flip from forehand to backhand when on the left half of the table
            // otherwhise flip back!
            // for more performance do only once
            if (Transform.position.x >= 0)
                Rigidbody.MoveRotation(Quaternion.Euler(Enemy.StartRotationAngles.x, 180, Enemy.StartRotationAngles.z));
            else
                Rigidbody.MoveRotation(Quaternion.Euler(Enemy.StartRotationAngles.x, 0, Enemy.StartRotationAngles.z));
        }
        
        private void Setup()
        {
            Actions = new List<Action>();
            EntryActions = new List<Action>();
            ExitActions = new List<Action>();
            Transitions = new List<ITransition>();
        }
    }
}