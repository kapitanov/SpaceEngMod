using Sandbox.ModAPI;
using VRageMath;

namespace SpaceEngMod
{
    public sealed class TrackPlayerProgram : IFlightProgram
    {
        private readonly IMyPlayer _player;
        private readonly PidController1D _yawPid = new PidController1D
        {
            D = 2f,
            P = -0.5f,
            I = 0
        };

        private readonly PidController1D _pitchPid = new PidController1D
        {
            D = 2f,
            P = -0.5f,
            I = 0
        };

        private readonly Value1D _yaw = new Value1D();
        private readonly Value1D _pitch = new Value1D();

        public TrackPlayerProgram(IMyPlayer player)
        {
            _player = player;
        }
        public string Name
        {
            get { return "TrackPlayer"; }
        }

        public void Initialize(IFlightContext ctx)
        {

        }

        public void Update(IFlightContext ctx, ref bool programTerminated)
        {
            var target = _player.GetPosition();
          
            var direction = target - ctx.Entity.GetPosition();
            direction.Normalize();

            var targetYaw = (float)ctx.Entity.WorldMatrix.Left.Dot(direction);
            var targetPitch = (float)ctx.Entity.WorldMatrix.Left.Dot(direction);

            _yaw.Update(targetYaw, ctx.DeltaT);
            _pitch.Update(ctx.Pitch.Value - targetPitch, ctx.DeltaT);


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
               /*
           
            var shipWorldMatrix = ctx.Entity.WorldMatrix;
            var oneVector = Vector3.One;

            float globalShipPitch, globalShipYaw, globalShipRoll;
            SMath.ComputePitchYawRoll(
                ref shipWorldMatrix,
                ref oneVector,
                out globalShipPitch,
                out globalShipYaw,
                out globalShipRoll
                );

            var playerWorldMatrix = MatrixD.CreateTranslation(target);
            
            float globalPlayerPitch, globalPlayerYaw, globalPlayerRoll;
            SMath.ComputePitchYawRoll(
                ref playerWorldMatrix,
                ref oneVector,
                out globalPlayerPitch,
                out globalPlayerYaw,
                out globalPlayerRoll
                );

            var direction = Vector3.Normalize(target - ctx.Entity.GetPosition());
            float relativePitch, relativeYaw, relativeRoll;
            SMath.ComputePitchYawRoll(
                ref shipWorldMatrix,
                ref direction,
                out relativePitch,
                out relativeYaw,
                out relativeRoll
                );
        
            ctx.Log(string.Format("Player at {0}, direction is {1}", target, direction));
            ctx.Log(string.Format(
               "gship = {{{0:0.000}, {1:0.000}, {2:0.000}}}, " +
               "gplayer = {{{3:0.000}, {4:0.000}, {5:0.000}}}, " +
               "rel = {{{6:0.000}, {7:0.000}, {8:0.000}}}, " +
               "cmp = {{{9:0.000}, {10:0.000}, {11:0.000}}}",
               globalShipPitch, globalShipYaw, globalShipRoll,
               globalPlayerPitch, globalPlayerYaw, globalPlayerRoll,
               relativePitch, relativeYaw, relativeRoll,
               globalShipPitch - globalPlayerPitch, globalShipYaw - globalPlayerYaw, globalShipRoll - globalPlayerRoll
               ));
 
            */


                  ctx.Controller.MoveAndRotate(Vector3.Zero, new Vector2(uPitch, uYaw), 0);



        }
    }
}