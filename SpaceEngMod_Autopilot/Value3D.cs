using VRageMath;

namespace SpaceEngMod
{
    public sealed class Value3D
    {
        public Vector3 Value { get; private set; }
        public Vector3 Differential { get; private set; }
        public Vector3 Integral { get; private set; }

        public void Update(Vector3 value, float dt)
        {
            Differential = (value - Value) / dt;
            Integral += value * dt;
            Value = value;
        }
    }
}