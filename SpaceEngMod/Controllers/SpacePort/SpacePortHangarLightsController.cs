using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox.Common;
using Sandbox.ModAPI;

using SPX.Station.Infrastructure.ApiEntities;
using SPX.Station.Infrastructure.ApiEntities.Enums;
using SPX.Station.Infrastructure.Events;
using SPX.Station.Infrastructure.Implementation.SpacePort;
using SPX.Station.Infrastructure.Utils;

namespace SPX.Station.Infrastructure.Controllers.SpacePort
{
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
    public sealed class SpacePortHangarLightsController : MySessionComponentBase
    {
        private bool _initialized;

        public override void UpdateBeforeSimulation()
        {
            if (!_initialized)
            {
                using (Log.Scope("SpacePortHangarLightsController.Initialize"))
                {
                   
                    EntityEvents.HangarLightsSensorStateChanged.Subscribe(EntityEvents_SensorStateChanged);
                    _initialized = true;
                }
            }

            base.UpdateBeforeSimulation();
        }

        
        private void EntityEvents_SensorStateChanged(Sensor sensor, bool state)
        {
            try
            {
                if (!sensor.Entity.CustomName.StartsWith(Constants.SpacePortPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                var hangarLight = GetHangarLight(sensor);
                if (hangarLight != null)
                {
                    if (state)
                    {
                        hangarLight.ToggleRed(this);
                    }
                    else
                    {
                        hangarLight.ToggleGreen(this);
                    }

                }
            }
            catch (Exception e)
            {
                Log.Error(e, "EntityEvents_SensorStateChanged");
            }
        }

        private readonly Dictionary<Sensor, HangarLight> _hangarLights = new Dictionary<Sensor, HangarLight>();

        private HangarLight GetHangarLight(Sensor sensor)
        {
            //Log.Write("GetHangarLight initiated {0}", sensor.Entity.CustomName);

            if (!sensor.Entity.CustomName.StartsWith(Constants.SpacePortPrefix, StringComparison.OrdinalIgnoreCase) || sensor.SensorType != SensorType.HangarLights)
            {
                return null;
            }

            HangarLight hangarLight;
            if (!_hangarLights.TryGetValue(sensor, out hangarLight))
            {
                hangarLight = new HangarLight(sensor);
                _hangarLights.Add(sensor, hangarLight);
            }

            return hangarLight;
        }
    }
}