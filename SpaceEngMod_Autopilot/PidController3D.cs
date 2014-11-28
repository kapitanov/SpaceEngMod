using VRageMath;

namespace SpaceEngMod
{
    public sealed class PidController3D
    {
        private Vector3 _x;
        private Vector3 _dx;
        private Vector3 _ix;
        private Vector3 _u;

        public PidController3D()
        {
            P = 1;
            D = 1;
            I = 1;
        }

        public Vector3 X { get { return _x; } }

        public Vector3 U { get { return _u; } }

        public float P { get; set; }
        public float I { get; set; }
        public float D { get; set; }

        public void ValueChanged(Vector3 x, float dt)
        {
            _dx = (x - _x) / dt;
            _ix += x * dt;
            _x = x;

            _u = P * x + I * _ix + D * _dx;
        }
    }
}