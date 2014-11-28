using System;
using VRageMath;

namespace SpaceEngMod
{
    public sealed class FlightToProgram : IFlightProgram
    {
        private readonly Vector3 _target;
        private readonly PidController1D _yawPid = new PidController1D
        {
            D = 5f,
            P = 0.5f,
            I = 0
        };

        private readonly PidController1D _pitchPid = new PidController1D
        {
            D = 5f,
            P = 0.5f,
            I = 0
        };

        private readonly Value1D _yaw = new Value1D();
        private readonly Value1D _pitch = new Value1D();

        public FlightToProgram(Vector3 target)
        {
            _target = target;

        }

        public string Name
        {
            get { return string.Format("FlightTo({0})", _target); }
        }

        public void Initialize(IFlightContext ctx)
        {

        }

        public void Update(IFlightContext ctx, ref bool programTerminated)
        {

            var direction = _target - ctx.Entity.GetPosition();
            direction.Normalize();

            var targetYaw = -(float)ctx.Entity.WorldMatrix.Left.Dot(direction);
            var targetPitch = -(float)ctx.Entity.WorldMatrix.Left.Dot(direction);

            _yaw.Update(targetYaw, ctx.DeltaT);
            _pitch.Update(ctx.Pitch.Value - targetPitch, ctx.DeltaT);

            if (Math.Abs(_yaw.Differential) < 0.005 &&
                Math.Abs(_pitch.Differential) < 0.005 &&
                Math.Abs(_yaw.Value) < 0.01 &&
                Math.Abs(_pitch.Value) < 0.01
                )
            {
                programTerminated = true;
                return;
            }



            var uYaw = _yawPid.Evaluate(_yaw);
            var uPitch = _pitchPid.Evaluate(_pitch);

            ctx.Log(string.Format(
                "yaw = {0:0.000}, d(yaw) = {1:0.000}, I(yaw) = {2:0.000}, target = {3:0.000}, YAW = {4:0.0} deg / {5:0.0} deg",
                ctx.Yaw.Value,
                ctx.Yaw.Differential,
                ctx.Yaw.Integral,
                targetYaw,
                MathHelper.ToDegrees(ctx.Yaw.Value),
                MathHelper.ToDegrees(targetYaw)
                ));
            ctx.Log(string.Format(
                "tyaw = {0:0.000}, d(tyaw) = {1:0.000}, I(tyaw) = {2:0.000}, u = {3:0.000}",
                _yaw.Value,
                _yaw.Differential,
                _yaw.Integral,
                uYaw
                ));


            ctx.Controller.MoveAndRotate(Vector3.Zero, new Vector2(uPitch, uYaw), 0);



        }
    }
}