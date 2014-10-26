using System;
using System.Linq;
using Sandbox.ModAPI;

namespace SpaceEngMod
{
    public sealed class AutoDoor
    {
        private readonly Sensor _sensor;

        public AutoDoor(Sensor sensor)
        {
            _sensor = sensor;
        }

        public AutoDoorState State { get; private set; }

        public void Open(AutoDoorController ctrl)
        {
            MoveDoor(ctrl, true);
        }

        public void Close(AutoDoorController ctrl)
        {
            MoveDoor(ctrl, false);
        }

        public void Toggle(AutoDoorController ctrl)
        {
            MoveDoor(ctrl, State != AutoDoorState.Open);
        }

        private void MoveDoor(AutoDoorController ctrl, bool open)
        {
            var options = new Options(_sensor.Entity.CustomName);

            var sensorPosition = _sensor.Entity.GetPosition();
            var distance = options.Get("R", 25f);
            var velocity = options.Get("V", 5f);

            // Locate all pistons within D meters
            var pistons = Entities.Pistons
                .Where(p => p.Entity.CustomName.StartsWith(AutoDoorController.Prefix, StringComparison.OrdinalIgnoreCase))
                .Where(p => p.Entity.CubeGrid == _sensor.Entity.CubeGrid)
                .Where(p => (p.Entity.GetPosition() - sensorPosition).Length() <= distance)
                .ToList();

            // Locate all landing gears within D meters
            var landingGears = Entities.LandingGears
                .Where(p => p.Entity.CustomName.StartsWith(AutoDoorController.Prefix, StringComparison.OrdinalIgnoreCase))
                .Where(p => p.Entity.CubeGrid == _sensor.Entity.CubeGrid)
                .Where(p => (p.Entity.GetPosition() - sensorPosition).Length() <= distance)
                .ToList();

            Log.Write("Got {0} pistons, {1} landing gears within {2}m while sensor is {3}", pistons.Count, landingGears.Count, distance,
                _sensor.Entity.CustomName);
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
                    piston.SetVelocity(-velocity);
                }

                Log.Write("AUTODOOR OPEN");
                State = AutoDoorState.Open;
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
                    piston.SetVelocity(velocity);
                }

                Log.Write("AUTODOOR CLOSED");
                State = AutoDoorState.Closed;
            }

            // When each of pistons reaches limit we will lock the corresponding (nearest) piston
            // See EntityEvents_PistonLimitReached
            Log.Write("AUTODOOR: setting _pendingLocks");
            ctrl.ClearPendingLandingGears();
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
                    ctrl.SetPendingLandingGear(piston, p.landingGear);
                    landingGears.RemoveAt(p.index);
                }
            }
        }
    }
}