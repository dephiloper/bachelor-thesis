using System;

namespace SampleDecisionTree
{
    public interface INode
    {
        INode MakeDecision();
    }
}