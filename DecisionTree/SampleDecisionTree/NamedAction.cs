using System;

namespace SampleDecisionTree
{
    public class NamedAction : ActionNode
    {
        private readonly string _name;

        public NamedAction(string name)
        {
            _name = name;
        }

        public override void Execute()
        {
            Console.WriteLine(_name);
        }
    }
}