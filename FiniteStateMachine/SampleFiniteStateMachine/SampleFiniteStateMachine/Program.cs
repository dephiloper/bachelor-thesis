using System.Collections.Generic;
using SampleFiniteStateMachine.Actions;
using SampleFiniteStateMachine.Conditions;

namespace SampleFiniteStateMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            var standState = new State
            {
                Actions = new[] {new PhraseAction("Stand")},
                EntryActions = new[] {new PhraseAction("Start Standing")},
                ExitActions = new[] {new PhraseAction("Stop Standing")}
            };

            var standTransition = new Transition
            {
                Actions = new[] {new PhraseAction("Stand Still")},
                Condition = new FloatCondition(1, 2, 1.5f),
                TargetState = standState
            };

            standState.Transitions = new[] {standTransition};
            
            var stateMachine = new StateMachine(standState)
            {
                States = new [] {standState}
            };

            stateMachine.Update();
        }
    }
}