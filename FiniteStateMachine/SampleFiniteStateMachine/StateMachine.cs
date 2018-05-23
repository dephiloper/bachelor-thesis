using System.Collections.Generic;
using System.Linq;
using SampleFiniteStateMachine.Interfaces;

namespace SampleFiniteStateMachine
{
    public class StateMachine
    {
        public IEnumerable<State> States { get; set; }
        public State CurrentState { get; set; }

        public StateMachine(State initialState)
        {
            CurrentState = initialState;
        }

        public IEnumerable<IAction> Update()
        {
            var triggeredTransition = CurrentState.Transitions.FirstOrDefault(x => x.IsTriggered());
            
            if (triggeredTransition == null) 
                return CurrentState.Actions;
            
            var targetState = triggeredTransition.TargetState;
            var actions = CurrentState.ExitActions.ToList();
            actions.AddRange(triggeredTransition.Actions);
            actions.AddRange(targetState.EntryActions);

            CurrentState = targetState;
            return actions;

        }
    }
}