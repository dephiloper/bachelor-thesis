using System;
using System.Collections.Generic;
using System.Linq;
using FiniteStateMachine.States;
using Interfaces;

namespace FiniteStateMachine
{
    public class StateMachine : IIntelligence
    {
        private BaseState CurrentState { get; set; }

        public StateMachine(BaseState initialState)
        {
            CurrentState = initialState;
        }

        public List<Action> Update()
        {
            var triggeredTransition = CurrentState.Transitions.FirstOrDefault(x => x.IsTriggered());
            
            if (triggeredTransition == null) 
                return CurrentState.Actions;
        
            var targetState = triggeredTransition.TargetState;
            var actions = new List<Action>();
            actions.AddRange(CurrentState.ExitActions);
            actions.AddRange(triggeredTransition.Actions);
            actions.AddRange(targetState.EntryActions);

            CurrentState = targetState;
            return actions;

        }
    }
}