using System;

namespace SampleDecisionTree
{
    class Program
    {
        private static void Main(string[] args)
        {
            var floatDecision = new FloatDecision
            {
                MinValue = 10,
                MaxValue = 200,
                DecisionValue = 120,
                TrueNode = new NamedAction("1"),
                FalseNode = new NamedAction("2")
            };
            
            var floatDecision2 = new FloatDecision
            {
                MinValue = -3,
                MaxValue = 100,
                DecisionValue = -5,
                TrueNode = new NamedAction("3"),
                FalseNode = new NamedAction("4")
            };

            var boolDecision = new BoolDecision
            {
                DecisionValue = false,
                TrueNode = floatDecision,
                FalseNode = floatDecision2
            };

            var action = boolDecision.MakeDecision() as ActionNode;
            action?.Execute();
        }
    }
}