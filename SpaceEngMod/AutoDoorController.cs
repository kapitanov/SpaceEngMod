using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.Common;
using Sandbox.ModAPI;

namespace SpaceEngMod
{
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
    public sealed class AutoDoorController : MySessionComponentBase
    {
        private const float Distance = 25;
        private const string Prefix = "D:";

        private bool _initialized;

        public override void UpdateBeforeSimulation()
        {
            if (!_initialized)
            {
                using (Log.Scope("AutoDoorController.Initialize"))
                {
                    EntityEvents.ButtonPressed.Subscribe(EntityEvents_ButtonPressed);
                    EntityEvents.SensorStateChanged.Subscribe(EntityEvents_SensorStateChanged);
                    EntityEvents.PistonLimitReached.Subscribe(EntityEvents_PistonLimitReached);
                    _initialized = true;
                }
            }

            base.UpdateBeforeSimulation();
        }

        private void EntityEvents_ButtonPressed(ButtonPanel buttonPanel, int button)
        {
            var buttonPanelPosition = buttonPanel.Entity.GetPosition();

            // Locate a sensor within D meters
            var sensor = Entities.Sensors
                .Where(p => p.Entity.CustomName.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault(p => (p.Entity.GetPosition() - buttonPanelPosition).Length() <= Distance);

            if (sensor != null)
            {
                AutodoorHandler(sensor, button == 0 || button == 1);
            }
            else
            {
                Log.Write("AUTODOOR: there is no sensor near this button panel");
            }
        }

        private void EntityEvents_SensorStateChanged(Sensor sensor, bool state)
        {
            if (!sensor.Entity.CustomName.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            AutodoorHandler(sensor, state);
        }

        private void AutodoorHandler(Sensor sensor, bool open)
        {
            var sensorPosition = sensor.Entity.GetPosition();

            // Locate all pistons within D meters
            var pistons = Entities.Pistons
                .Where(p => p.Entity.CustomName.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase))
                .Where(p => (p.Entity.GetPosition() - sensorPosition).Length() <= Distance)
                .ToList();

            // Locate all landing gears within D meters
            var landingGears = Entities.LandingGears
                .Where(p => p.Entity.CustomName.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase))
                .Where(p => (p.Entity.GetPosition() - sensorPosition).Length() <= Distance)
                .ToList();

            Log.Write("AUTODOOR");
            Log.Write("Got {0} pistons, {1} landing gears within {2}m while sensor is {3}", pistons.Count, landingGears.Count, Distance,
                sensor.Entity.CustomName);

            //var minPistDist = pistons.Min(p => (p.Entity.GetPosition() - sensorPosition).Length());
            //var maxPistDist = pistons.Max(p => (p.Entity.GetPosition() - sensorPosition).Length());
            //Log.Write("PistDist : [{0}m - {1}m]", minPistDist, maxPistDist);

            //var minLgDist = landingGears.Min(p => (p.Entity.GetPosition() - sensorPosition).Length());
            //var maxLgDist = landingGears.Max(p => (p.Entity.GetPosition() - sensorPosition).Length());
            //Log.Write("LgDist : [{0}m - {1}m]", minLgDist, maxLgDist);

            if (open)
            {
                // ======= OPEN DOOR =======

                MyAPIGateway.Utilities.ShowNotification("Autodoor: Opening");

                foreach (var landingGear in landingGears)
                {
                    landingGear.Unlock();
                }

                foreach (var piston in pistons)
                {
                    piston.SetVelocity(-5);
                }

                Log.Write("AUTODOOR OPEN");
            }
            else
            {
                // ======= CLOSE DOOR =======

                MyAPIGateway.Utilities.ShowNotification("Autodoor: Closing");

                foreach (var landingGear in landingGears)
                {
                    landingGear.Unlock();
                }

                foreach (var piston in pistons)
                {
                    piston.SetVelocity(5);
                }

                Log.Write("AUTODOOR CLOSE");
            }

            // When each of pistons reaches limit we will lock the corresponding (nearest) piston
            // See EntityEvents_PistonLimitReached
            _pendingLocks.Clear();
            foreach (var piston in pistons)
            {
                var pistonPosition = piston.Entity.GetPosition();

                var p = landingGears.Select((landingGear, index) => new
                {
                    landingGear,
                    D = (landingGear.Entity.GetPosition() - pistonPosition).Length(),
                    index
                })
                    .OrderBy(_ => _.D)
                    .FirstOrDefault();

                if (p != null)
                {
                    _pendingLocks[piston] = p.landingGear;
                    landingGears.RemoveAt(p.index);
                }
            }
        }

        private void EntityEvents_PistonLimitReached(Piston piston, bool arg2)
        {
            if (!piston.Entity.CustomName.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase))
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

        private readonly Dictionary<Piston, LandingGear> _pendingLocks = new Dictionary<Piston, LandingGear>();
    }
}
