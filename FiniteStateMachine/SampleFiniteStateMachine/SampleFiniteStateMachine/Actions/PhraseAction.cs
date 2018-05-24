using System;
using SampleFiniteStateMachine.Interfaces;

namespace SampleFiniteStateMachine.Actions
{
    public class PhraseAction : IAction
    {
        private string _phrase; 
        
        public PhraseAction(string phrase)
        {
            _phrase = phrase;
        }
        
        public void Execute()
        {
            Console.WriteLine($"phrase: {_phrase}");
        }
    }
}