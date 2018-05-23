namespace SampleDecisionTree
{
    public class FloatDecision : DecisionNode
    {
        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public float DecisionValue { get; set; }
        
        protected override INode GetBranch()
        {
            if (MinValue < DecisionValue && DecisionValue < MaxValue)
                return TrueNode;

            return FalseNode;
        }
    }
}