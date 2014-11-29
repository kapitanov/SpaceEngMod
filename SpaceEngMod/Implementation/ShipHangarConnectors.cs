using System;
using System.Linq;

using Sandbox.ModAPI;

using SPX.Station.Infrastructure.ApiEntities;
using SPX.Station.Infrastructure.Controllers;
using SPX.Station.Infrastructure.Utils;

namespace SPX.Station.Infrastructure.Implementation
{
    public class ShipHangarConnectors
    {
        private readonly ButtonPanel _button;

        public HangarState HangarConnectorsState { get; private set; }

        public ShipHangarConnectors(ButtonPanel button)
        {
            _button = button;
            HangarConnectorsState = HangarState.Closed;
        }

        public void Open(SpacePortConnectorsController ctrl)
        {
            SwitchGearsLock(ctrl, true);
        }

        private void SwitchGearsLock(SpacePortConnectorsController ctrl, bool toOpen)
        {
            Log.Write("There is {0} connectors totally", Entities.Connectors.Count);
            Log.Write("There is {0} connectors in group {1} totally", Entities.Connectors.Count(p => p.HangarCode == _button.HangarCode), _button.HangarCode);

            var connectors =
                Entities.Connectors.Where(
                    p => p.Entity.CustomName.StartsWith(Constants.SpacePortPrefix, StringComparison.OrdinalIgnoreCase))
                    .Where(p => p.HangarCode.Equals(_button.HangarCode, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            Log.Write("Got {0} connectors, while button is {1}", connectors.Count, _button.Entity.CustomName);

            if (toOpen)
            {
                MyAPIGateway.Utilities.ShowNotification("Landing Gears: Unlocking");

                foreach (var connector in connectors)
                {
                    connector.Unlock();
                }

                Log.Write("HANGAR GEARS UNLOCKED");
                HangarConnectorsState = HangarState.Open;
            }
            else
            {
                MyAPIGateway.Utilities.ShowNotification("Landing Gears: Locking");

                foreach (var connector in connectors)
                {
                    connector.Unlock();
                }

                Log.Write("HANGAR GEARS LOCKED");
                HangarConnectorsState = HangarState.Closed;
            }
        }

        public void Close(SpacePortConnectorsController ctrl)
        {
            SwitchGearsLock(ctrl, false);
        }

        public void Toggle(SpacePortConnectorsController ctrl)
        {
            SwitchGearsLock(ctrl, HangarConnectorsState != HangarState.Open);
        }
    }
}