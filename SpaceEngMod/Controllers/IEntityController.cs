using Sandbox.ModAPI.Ingame;

namespace SPX.Station.Infrastructure.Controllers
{
    public interface IEntityController
    {
        IMyTerminalBlock Entity { get; }
        string EntityName { get; }
        string EntityTypeName { get; }
        bool HasEntity { get; }
    }
}