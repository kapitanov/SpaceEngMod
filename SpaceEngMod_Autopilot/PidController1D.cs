namespace SpaceEngMod
{
    public sealed class PidController1D
    {
        public PidController1D()
        {
            P = 1;
            D = 1;
            I = 1;
        }

        public float P { get; set; }
        public float I { get; set; }
        public float D { get; set; }

        public float Evaluate(Value1D x)
        {
            var u = P * x.Value + I * x.Integral + D * x.Differential;
            return u;
        }
    }
}