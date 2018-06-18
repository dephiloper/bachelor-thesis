using System;
using System.Collections.Generic;
using FiniteStateMachine.Interfaces;
using FiniteStateMachine.States;

namespace FiniteStateMachine
{
    public class Transition : ITransition
    {
        public BaseState TargetState { get; set; }
        public Func<bool> Condition { private get; set; }
        public List<Action> Actions { get; set; } = new List<Action>();
        public bool Negate { get; set; } = false;  
    
        public bool IsTriggered() => Negate ? !Condition() : Condition();

        public Transition(BaseState targetState, Func<bool> condition, bool negate = false)
        {
            TargetState = targetState;
            Condition = condition;
            Negate = negate;
        }
    }
}