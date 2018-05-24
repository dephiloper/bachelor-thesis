using System.Collections.Generic;
using SampleFiniteStateMachine.Interfaces;

namespace SampleFiniteStateMachine
{
    public class Transition : ITransition
    {
        public State TargetState { get; set; }
        public IEnumerable<IAction> Actions { get; set; }
        public ICondition Condition { get; set; }
        
        public bool IsTriggered() => Condition.Evaluate();
    }
}