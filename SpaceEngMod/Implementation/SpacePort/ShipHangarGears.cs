using System;
using System.Linq;

using Sandbox.ModAPI;

using SPX.Station.Infrastructure.ApiEntities;
using SPX.Station.Infrastructure.Controllers.SpacePort;
using SPX.Station.Infrastructure.Utils;

namespace SPX.Station.Infrastructure.Implementation.SpacePort
{
    public class ShipHangarGears
    {
        private readonly ButtonPanel _button;

        public HangarState HangarGearsState { get; private set; }

        public ShipHangarGears(ButtonPanel button)
        {
            _button = button;
            HangarGearsState = HangarState.Closed;

            Log.Write("Hangar {0} gears created: ", button.HangarCode);
        }

        public void Open(SpacePortGearsController ctrl)
        {
            SwitchGearsLock(ctrl, true);
        }

        private void SwitchGearsLock(SpacePortGearsController ctrl, bool toOpen)
        {
            //Log.Write("There is {0} landing gears totally", Entities.LandingGears.Count);
            //Log.Write("There is {0} landing gears in group {1} totally", Entities.LandingGears.Count(p => p.HangarCode == _button.HangarCode), _button.HangarCode);

            var gears =
                Entities.LandingGears.Where(
                    p => p.Entity.CustomName.StartsWith(Constants.SpacePortPrefix, StringComparison.OrdinalIgnoreCase))
                    .Where(p => p.HangarCode.Equals(_button.HangarCode, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            Log.Write("Got {0} landing gears, while button is {1}", gears.Count, _button.Entity.CustomName);

            if (toOpen)
            {
                MyAPIGateway.Utilities.ShowNotification("Landing Gears: Unlocking");

                foreach (var gear in gears)
                {
                    gear.Unlock();
                }

                Log.Write("HANGAR GEARS UNLOCKED");
                HangarGearsState = HangarState.Open;
            }
            else
            {
                MyAPIGateway.Utilities.ShowNotification("Landing Gears: Locking");

                foreach (var gear in gears)
                {
                    gear.Lock();
                }

                Log.Write("HANGAR GEARS LOCKED");
                HangarGearsState = HangarState.Closed;
            }
        }

        public void Close(SpacePortGearsController ctrl)
        {
            SwitchGearsLock(ctrl, false);
        }

        public void Toggle(SpacePortGearsController ctrl)
        {
            SwitchGearsLock(ctrl, HangarGearsState != HangarState.Open);
        }
    }
}