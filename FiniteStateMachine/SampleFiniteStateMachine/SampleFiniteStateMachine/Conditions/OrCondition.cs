using SampleFiniteStateMachine.Interfaces;

namespace SampleFiniteStateMachine.Conditions
{
    public class OrCondition : BooleanCondition
    {
        public OrCondition(ICondition conditionLeft, ICondition conditionRight) : base(conditionLeft, conditionRight)
        {
        }

        public override bool Evaluate() => ConditionLeft.Evaluate() || ConditionRight.Evaluate();
    }
}