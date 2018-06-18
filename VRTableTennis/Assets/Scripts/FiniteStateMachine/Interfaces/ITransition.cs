using System;
using System.Collections.Generic;
using FiniteStateMachine.States;

namespace FiniteStateMachine.Interfaces
{
    public interface ITransition
    {
        BaseState TargetState { get; set; }
        List<Action> Actions { get; set; }

        bool IsTriggered();
    }
}