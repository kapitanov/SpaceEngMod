using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox.Common;
using Sandbox.ModAPI;

using SPX.Station.Infrastructure.ApiEntities;
using SPX.Station.Infrastructure.Events;
using SPX.Station.Infrastructure.Implementation;
using SPX.Station.Infrastructure.Utils;

namespace SPX.Station.Infrastructure.Controllers
{
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
    public sealed class AutoDoorController : MySessionComponentBase
    {
        private const float Distance = 25;
        
        private bool _initialized;

        public override void UpdateBeforeSimulation()
        {
            if (!_initialized)
            {
                using (Log.Scope("AutoDoorController.Initialize"))
                {
                    EntityEvents.AutoDoorButtonPressed.Subscribe(EntityEvents_ButtonPressed);
                    EntityEvents.SensorStateChanged.Subscribe(EntityEvents_SensorStateChanged);
                    EntityEvents.PistonLimitReached.Subscribe(EntityEvents_PistonLimitReached);
                    EntityEvents.AutoDoorButtonUpdate100.Subscribe(EntityEvents_ButtonUpdate10);
                    _initialized = true;
                }
            }

            base.UpdateBeforeSimulation();
        }

        private void EntityEvents_ButtonPressed(ButtonPanel buttonPanel, int button)
        {
            try
            {
                var autodoor = GetAutodoor(buttonPanel);
                if (autodoor != null)
                {
                    autodoor.Toggle(this);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "EntityEvents_ButtonPressed");
            }
        }

        private void EntityEvents_SensorStateChanged(Sensor sensor, bool state)
        {
            try
            {
                if (!sensor.Entity.CustomName.StartsWith(Constants.AutoDoorPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                var autodoor = GetAutodoor(sensor);
                if (autodoor != null)
                {
                    if (state)
                    {
                        autodoor.Open(this);
                    }
                    else
                    {
                        autodoor.Close(this);
                    }

                }
            }
            catch (Exception e)
            {
                Log.Error(e, "EntityEvents_SensorStateChanged");
            }
        }

        private bool _notificationShown;

        private void EntityEvents_ButtonUpdate10(ButtonPanel buttonPanel)
        {
            try
            {
                if ((MyAPIGateway.Session.Player.GetPosition() - buttonPanel.Entity.GetPosition()).Length() < 5)
                {
                    var autodoor = GetAutodoor(buttonPanel);
                    if (autodoor != null)
                    {
                        if (!_notificationShown)
                        {
                            MyAPIGateway.Utilities.ShowNotification("This button controls autodoor", 1000, MyFontEnum.Green);
                            _notificationShown = true;
                        }
                    }
                }
                else
                {
                    _notificationShown = false;
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "EntityEvents_SensorStateChanged");
            }
        }

        public void ClearPendingLandingGears()
        {
            _pendingLocks.Clear();
        }

        public void SetPendingLandingGear(AbstractPiston piston, LandingGear landingGear)
        {
            _pendingLocks[piston] = landingGear;
        }


        private void EntityEvents_PistonLimitReached(AbstractPiston piston, bool arg2)
        {
            try
            {
                if (!piston.Entity.CustomName.StartsWith(Constants.AutoDoorPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                LandingGear landingGear;
                if (_pendingLocks.TryGetValue(piston, out landingGear))
                {
                    _pendingLocks.Remove(piston);
                    landingGear.Lock();
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "EntityEvents_PistonLimitReached");
            }
        }

        private readonly Dictionary<AbstractPiston, LandingGear> _pendingLocks = new Dictionary<AbstractPiston, LandingGear>();
        private readonly Dictionary<Sensor, AutoDoor> _autoDoors = new Dictionary<Sensor, AutoDoor>();

        private AutoDoor GetAutodoor(Sensor sensor)
        {
            if (!sensor.Entity.CustomName.StartsWith(Constants.AutoDoorPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            AutoDoor autodoor;
            if (!_autoDoors.TryGetValue(sensor, out autodoor))
            {
                autodoor = new AutoDoor(sensor);
                _autoDoors.Add(sensor, autodoor);
            }

            return autodoor;
        }

        private AutoDoor GetAutodoor(ButtonPanel buttonPanel)
        {
            if (!buttonPanel.Entity.CustomName.StartsWith(Constants.AutoDoorPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            // Locate a sensor within D meters within the same grid
            var buttonPanelPosition = buttonPanel.Entity.GetPosition();
            var sensor = Entities.Sensors
                .Where(p => p.Entity.CustomName.StartsWith(Constants.AutoDoorPrefix, StringComparison.OrdinalIgnoreCase))
                .Where(p => p.Entity.CubeGrid == buttonPanel.Entity.CubeGrid)
                .FirstOrDefault(p => (p.Entity.GetPosition() - buttonPanelPosition).Length() <= Distance);

            if (sensor != null)
            {
                var autodoor = GetAutodoor(sensor);
                return autodoor;
            }

            MyAPIGateway.Utilities.ShowNotification(
                string.Format("AUTODOOR: there is no sensor with name prefix '{0}' nearby", Constants.AutoDoorPrefix),
                font: MyFontEnum.Red);
            Log.Write("AUTODOOR: there is no sensor nearby this button panel");
            return null;
        }
    }
}
