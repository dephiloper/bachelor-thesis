using System;
using System.Collections.Generic;
using FiniteStateMachine.Interfaces;

namespace FiniteStateMachine
{
    public class State
    {
        public List<Action> Actions { get; set; }
        public List<Action> EntryActions { get; set; }
        public List<Action> ExitActions { get; set; }
        public List<ITransition> Transitions { get; set; }

        public State()
        {
            Actions = new List<Action>();
            EntryActions = new List<Action>();
            ExitActions = new List<Action>();
            Transitions = new List<ITransition>();
        }
    }
}