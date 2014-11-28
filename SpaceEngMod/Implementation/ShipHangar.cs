using System;
using System.Linq;

using Sandbox.ModAPI;

using SPX.Station.Infrastructure.ApiEntities;
using SPX.Station.Infrastructure.Controllers;
using SPX.Station.Infrastructure.Utils;

namespace SPX.Station.Infrastructure.Implementation
{
    public class ShipHangar
    {
        private readonly ButtonPanel _button;

        public HangarState HangarState { get; private set; }

        public ShipHangar(ButtonPanel button)
        {
            _button = button;
        }

        public void Open(SpacePortPistonController ctrl)
        {
            MovePlatform(ctrl, true);
        }

        private void MovePlatform(SpacePortPistonController ctrl, bool toOpen)
        {
            var options = new Options(_button.Entity.CustomName);

            var distance = options.Get("R", 25f);
            var velocity = options.Get("V", 1f);

            Log.Write("There is {0} pistons totally", Entities.Pistons.Count);
            Log.Write("There is {0} pistons in group {1} totally", Entities.Pistons.Count(p => p.HangarCode == _button.HangarCode), _button.HangarCode);

            var pistons =
                Entities.Pistons.Where(
                    p => p.Entity.CustomName.StartsWith(Constants.SpacePortPrefix, StringComparison.OrdinalIgnoreCase))
                    .Where(p => p.HangarCode.Equals(_button.HangarCode, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            Log.Write("Got {0} pistons, within {1}m while button is {2}", pistons.Count, distance, _button.Entity.CustomName);

            if (toOpen)
            {
                MyAPIGateway.Utilities.ShowNotification("Hangar: Opening");

                foreach (var piston in pistons)
                {
                    piston.SetVelocity(-velocity);
                }

                Log.Write("HANGAR OPEN");
                HangarState = HangarState.Open;
            }
            else
            {
                // ======= CLOSE DOOR =======

                MyAPIGateway.Utilities.ShowNotification("Hangar: Closing");

                foreach (var piston in pistons)
                {
                    piston.SetVelocity(velocity);
                }

                Log.Write("HANGAR CLOSED");
                HangarState = HangarState.Closed;
            }
        }

        public void Close(SpacePortPistonController ctrl)
        {
            MovePlatform(ctrl, false);
        }

        public void Toggle(SpacePortPistonController ctrl)
        {
            MovePlatform(ctrl, HangarState != HangarState.Open);
        }
    }
}
