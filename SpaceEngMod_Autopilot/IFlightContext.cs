using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;

namespace SpaceEngMod
{
    public interface IFlightContext
    {
        IMyCubeGrid Ship { get; }
        IMyEntity Entity { get; }
        IMyControllableEntity Controller { get; }
        
        Value3D Position { get; }
        Value1D Pitch { get; }
        Value1D Yaw { get; }
        Value1D Roll { get; }

        Value1D Velocity { get; }
        Value1D Acceleration { get; }
        float DeltaT { get; }

        void Log(string message);
    }
}