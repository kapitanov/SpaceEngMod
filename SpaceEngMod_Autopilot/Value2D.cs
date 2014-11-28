using VRageMath;

namespace SpaceEngMod
{
    public sealed class Value2D
    {
        public Vector2 Value { get; private set; }
        public Vector2 Differential { get; private set; }
        public Vector2 Integral { get; private set; }

        public void Update(Vector2 value, float dt)
        {
            Differential = (value - Value) / dt;
            Integral += value * dt;
            Value = value;
        }
    }
}