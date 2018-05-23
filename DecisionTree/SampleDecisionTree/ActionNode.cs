namespace SampleDecisionTree
{
    public abstract class ActionNode : INode
    {
        public abstract void Execute();

        public INode MakeDecision() => this;
    }
}