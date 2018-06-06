using System;
using System.Collections.Generic;

namespace FiniteStateMachine.Interfaces
{
    public interface ITransition
    {
        State TargetState { get; set; }
        List<Action> Actions { get; set; }

        bool IsTriggered();
    }
}