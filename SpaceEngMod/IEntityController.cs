using Sandbox.ModAPI.Ingame;

namespace SpaceEngMod
{
    public interface IEntityController
    {
        IMyTerminalBlock Entity { get; }
        string EntityName { get; }
        string EntityTypeName { get; }
        bool HasEntity { get; }
    }
}