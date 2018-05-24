using System.Collections.Generic;

namespace SampleFiniteStateMachine.Interfaces
{
    public interface ITransition
    {
        State TargetState { get; set; }
        IEnumerable<IAction> Actions { get; set; }

        bool IsTriggered();
    }
}