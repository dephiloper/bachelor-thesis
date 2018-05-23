using System.Collections.Generic;
using SampleFiniteStateMachine.Interfaces;

namespace SampleFiniteStateMachine
{
    public class State
    {
        public IEnumerable<IAction> Actions { get; set; }
        public IEnumerable<IAction> EntryActions { get; set; }
        public IEnumerable<IAction> ExitActions { get; set; }
        public IEnumerable<ITransition> Transitions { get; set; }
    }
}