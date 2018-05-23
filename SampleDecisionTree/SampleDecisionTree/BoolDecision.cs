namespace SampleDecisionTree
{
    public class BoolDecision : DecisionNode
    {
        public bool DecisionValue { get; set; }
        
        protected override INode GetBranch()
        {
            return DecisionValue ? TrueNode : FalseNode;
        }
    }
}