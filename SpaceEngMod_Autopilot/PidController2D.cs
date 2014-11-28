using VRageMath;

namespace SpaceEngMod
{
    public sealed class PidController2D
    {
        private Vector2 _x;
        private Vector2 _dx;
        private Vector2 _ix;
        private Vector2 _u;

        public PidController2D()
        {
            P = 1;
            D = 1;
            I = 1;
        }

        public Vector2 X { get { return _x; } }

        public Vector2 U { get { return _u; } }

        public float P { get; set; }
        public float I { get; set; }
        public float D { get; set; }

        public void ValueChanged(Vector2 x, float dt)
        {
            _dx = (x - _x) / dt;
            _ix += x * dt;
            _x = x;

            _u = P * x + I * _ix + D * _dx;
        }
    }
}