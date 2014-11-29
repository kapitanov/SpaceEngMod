using System;
using System.Linq;

using Sandbox.ModAPI;

using SPX.Station.Infrastructure.ApiEntities;
using SPX.Station.Infrastructure.ApiEntities.Enums;
using SPX.Station.Infrastructure.Controllers.SpacePort;
using SPX.Station.Infrastructure.Utils;

namespace SPX.Station.Infrastructure.Implementation.SpacePort
{
    public class HangarLight
    {
        private readonly Sensor _sensor;

        public HangarLight(Sensor sensor)
        {
            _sensor = sensor;
        }

        public void ToggleRed(SpacePortHangarLightsController ctrl)
        {
            ChangeLights(ctrl, SignalType.Busy);
        }

        private void ChangeLights(SpacePortHangarLightsController ctrl, SignalType signalType)
        {

            Log.Write("There is {0} lights totally", Entities.InteriorLights.Count);
            Log.Write("There is {0} pistons in group {1} totally", Entities.InteriorLights.Count(p => p.HangarCode == _sensor.HangarCode), _sensor.HangarCode);
            Log.Write("There is {0} lights totally with signalType {1}", Entities.InteriorLights.Count(i => i.HangarCode == _sensor.HangarCode && i.SignalType == signalType), signalType);
            Log.Write("There is {0} lights totally with other signalType than {1}", Entities.InteriorLights.Count(i => i.HangarCode == _sensor.HangarCode && i.SignalType != signalType), signalType);

            var lightsToTurnOn =
                Entities.InteriorLights.Where(
                    p => p.Entity.CustomName.StartsWith(Constants.SpacePortPrefix, StringComparison.OrdinalIgnoreCase))
                    .Where(p => p.HangarCode.Equals(_sensor.HangarCode, StringComparison.OrdinalIgnoreCase))
                    .Where(p => p.SignalType == signalType)
                    .ToList();

            var lightsToTurnOf =
                Entities.InteriorLights.Where(
                    p => p.Entity.CustomName.StartsWith(Constants.SpacePortPrefix, StringComparison.OrdinalIgnoreCase))
                    .Where(p => p.HangarCode.Equals(_sensor.HangarCode, StringComparison.OrdinalIgnoreCase))
                    .Where(p => p.SignalType != signalType)
                    .ToList();

            foreach (var interiorLight in lightsToTurnOn)
            {
                interiorLight.TurnOn();
            }

            foreach (var interiorLight in lightsToTurnOf)
            {
                interiorLight.TurnOff();
            }

            MyAPIGateway.Utilities.ShowNotification(string.Format("Hangar with Code is {0}", signalType));
        }

        public void ToggleGreen(SpacePortHangarLightsController ctrl)
        {
            ChangeLights(ctrl, SignalType.Free);
        }
    }
}