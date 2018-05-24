using SampleFiniteStateMachine.Interfaces;

namespace SampleFiniteStateMachine.Conditions
{
    public abstract class BooleanCondition : ICondition
    {
        protected ICondition ConditionLeft { get; }
        protected ICondition ConditionRight { get; }

        protected BooleanCondition(ICondition conditionLeft, ICondition conditionRight)
        {
            ConditionLeft = conditionLeft;
            ConditionRight = conditionRight;
        }

        public abstract bool Evaluate();
    }
}