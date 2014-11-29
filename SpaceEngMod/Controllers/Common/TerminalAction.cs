using System.Collections.Generic;
using System.Text;

using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;

using SPX.Station.Infrastructure.Utils;

namespace SPX.Station.Infrastructure.Controllers.Common
{
    public sealed class TerminalAction
    {
        // Generic
        public const string OnOff = "OnOff";
        public const string OnOff_On = "OnOff_On";
        public const string OnOff_Off = "OnOff_Off";

        // Piston
        public const string Reverse = "Reverse";
        public const string IncreaseVelocity = "IncreaseVelocity";
        public const string DecreaseVelocity = "DecreaseVelocity";
        public const string ResetVelocity = "ResetVelocity";
        public const string IncreaseUpperLimit = "IncreaseUpperLimit";
        public const string DecreaseUpperLimit = "DecreaseUpperLimit";
        public const string IncreaseLowerLimit = "IncreaseLowerLimit";
        public const string DecreaseLowerLimit = "DecreaseLowerLimit";

        // Sensor
        public const string IncreaseLeft = "IncreaseLeft";
        public const string DecreaseLeft = "DecreaseLeft";
        public const string IncreaseRight = "IncreaseRight";
        public const string DecreaseRight = "DecreaseRight";
        public const string IncreaseBottom = "IncreaseBottom";
        public const string DecreaseBottom = "DecreaseBottom";
        public const string IncreaseTop = "IncreaseTop";
        public const string DecreaseTop = "DecreaseTop";
        public const string IncreaseBack = "IncreaseBack";
        public const string DecreaseBack = "DecreaseBack";
        public const string IncreaseFront = "IncreaseFront";
        public const string DecreaseFront = "DecreaseFront";
        public const string DetectPlayers = "Detect Players";
        public const string DetectFloatingObjects = "Detect Floating Objects";
        public const string DetectSmallShips = "Detect Small Ships";
        public const string DetectLargeShips = "Detect Large Ships";
        public const string DetectStations = "Detect Stations";
        public const string DetectAsteroids = "Detect Asteroids";

        // ButtonPanel
        public const string AnyoneCanUse = "AnyoneCanUse";

        // LandingGear
        public const string Lock = "Lock";
        public const string Unlock = "Unlock";
        public const string SwitchLock = "SwitchLock";
        public const string Autolock = "Autolock";
        public const string IncreaseBreakForce = "IncreaseBreakForce";
        public const string DecreaseBreakForce = "DecreaseBreakForce";

        private readonly IEntityController _parent;
        private readonly string _name;

        private ITerminalAction _action;

        public TerminalAction(IEntityController parent, string name)
        {
            _parent = parent;
            _name = name;
        }

        public void Apply()
        {
            Init();
            if (_action != null && _parent.HasEntity)
            {
                Log.Write("{0}#{1}::{2}", _parent.EntityTypeName, _parent.Entity.EntityId, _name);
                _action.Apply((Sandbox.ModAPI.IMyCubeBlock)_parent.Entity);
            }
        }

        public bool IsEnabled(IMyCubeBlock block)
        {
            Init();
            if (_action != null && _parent.HasEntity)
            {
                return _action.IsEnabled((Sandbox.ModAPI.IMyCubeBlock)_parent.Entity);
            }

            return false;
        }

        public void WriteValue(IMyCubeBlock block, StringBuilder appendTo)
        {
            Init();
            if (_action != null && _parent.HasEntity)
            {
                Log.Write("{0}#{1}::{1} <- {2}", _parent.EntityTypeName, _parent.Entity.EntityId, _name, appendTo);
                _action.WriteValue((Sandbox.ModAPI.IMyCubeBlock)_parent.Entity, appendTo);
            }
        }

        private void Init()
        {
            if (_action == null)
            {
                if (!_parent.HasEntity)
                {
                    return;
                }

                var actions = new List<ITerminalAction>();
                _parent.Entity.GetActions(actions, _ => _.Id == _name);

                if (actions.Count == 0)
                {
                    Log.Write("ERROR Unable to find terminal action {0} on entity {1}", _name, _parent.EntityTypeName, _parent.EntityName);
                }
                else
                {
                    _action = actions[0];
                }
            }
        }
    }
}
