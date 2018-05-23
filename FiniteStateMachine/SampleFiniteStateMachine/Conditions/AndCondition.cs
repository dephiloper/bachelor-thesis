using SampleFiniteStateMachine.Interfaces;

namespace SampleFiniteStateMachine.Conditions
{
    public class AndCondition : BooleanCondition
    {
        public AndCondition(ICondition conditionLeft, ICondition conditionRight) : base(conditionLeft, conditionRight){}

        public override bool Evaluate() => ConditionLeft.Evaluate() && ConditionRight.Evaluate();
    }
}