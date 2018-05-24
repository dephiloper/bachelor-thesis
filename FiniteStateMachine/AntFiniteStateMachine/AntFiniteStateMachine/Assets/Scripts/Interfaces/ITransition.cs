using System;
using System.Collections.Generic;

namespace Interfaces
{
    public interface ITransition
    {
        State TargetState { get; set; }
        List<Action> Actions { get; set; }

        bool IsTriggered();
    }
}