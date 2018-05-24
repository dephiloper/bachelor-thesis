using SampleFiniteStateMachine.Interfaces;

namespace SampleFiniteStateMachine.Conditions
{
    public class FloatCondition : ICondition
    {
        private readonly float _minValue;
        private readonly float _maxValue;
        private readonly float _testValue;

        public FloatCondition(float minValue, float maxValue, float testValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _testValue = testValue;
        }

        public bool Evaluate() => _minValue <= _testValue && _testValue <= _maxValue;
    }
}