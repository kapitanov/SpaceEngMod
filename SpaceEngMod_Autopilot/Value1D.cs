namespace SpaceEngMod
{
    public sealed class Value1D
    {
        private bool _isFirstStep =true;

        public float Value { get; private set; }
        public float Differential { get; private set; }
        public float Integral { get; private set; }

        public void Update(float value, float dt)
        {
            if (!_isFirstStep)
            {
                Differential = (value - Value)/dt;
                Integral += value*dt;
            }

            Value = value;
            _isFirstStep = false;
        }
    }
}