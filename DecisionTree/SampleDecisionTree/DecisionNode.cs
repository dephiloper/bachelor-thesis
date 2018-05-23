using System;

namespace SampleDecisionTree
{
    public abstract class DecisionNode : INode
    {
        public INode TrueNode { get; set; }
        public INode FalseNode { get; set; }

        protected abstract INode GetBranch();

        public INode MakeDecision() => GetBranch().MakeDecision();
    }
}