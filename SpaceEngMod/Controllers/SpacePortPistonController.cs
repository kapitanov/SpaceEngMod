using System;
using System.Collections.Generic;

using Sandbox.Common;
using Sandbox.ModAPI;

using SPX.Station.Infrastructure.ApiEntities;
using SPX.Station.Infrastructure.Events;
using SPX.Station.Infrastructure.Implementation;
using SPX.Station.Infrastructure.Utils;

namespace SPX.Station.Infrastructure.Controllers
{
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
    public sealed class SpacePortPistonController : MySessionComponentBase
    {
        private const float Distance = 25;
        
        private bool _initialized;

        public override void UpdateBeforeSimulation()
        {
            if (!_initialized)
            {
                using (Log.Scope("SpacePortPistonController.Initialize Version 1.3 \r\n"))
                {
                    EntityEvents.PistonsButtonPressed.Subscribe(EntityEvents_ButtonPressed);
                    EntityEvents.PistonsButtonUpdate100.Subscribe(EntityEvents_ButtonUpdate100);
                    _initialized = true;
                }
            }

            base.UpdateBeforeSimulation();
        }

        private void EntityEvents_ButtonPressed(ButtonPanel buttonPanel, int button)
        {
            try
            {
                var depot = GetDepot(buttonPanel);
                if (depot != null)
                {
                    depot.Toggle(this);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "EntityEvents_ButtonPressed");
            }
        }

        
        private bool _notificationShown;

        private void EntityEvents_ButtonUpdate100(ButtonPanel buttonPanel)
        {
            try
            {
                if ((MyAPIGateway.Session.Player.GetPosition() - buttonPanel.Entity.GetPosition()).Length() < 5)
                {
                    var autodoor = GetDepot(buttonPanel);
                    if (autodoor != null)
                    {
                        if (!_notificationShown)
                        {
                            MyAPIGateway.Utilities.ShowNotification("This button controls depot door", 1000, MyFontEnum.Green);
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

        private readonly Dictionary<ButtonPanel, ShipHangar> _autoDoors = new Dictionary<ButtonPanel, ShipHangar>();

        private ShipHangar GetDepot(ButtonPanel button)
        {
            Log.Write("GetDepot initiated {0}", button.Entity.CustomName);
            if (!button.Entity.CustomName.StartsWith(Constants.SpacePortPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            ShipHangar shipHangar;
            if (!_autoDoors.TryGetValue(button, out shipHangar))
            {
                shipHangar = new ShipHangar(button);
                _autoDoors.Add(button, shipHangar);
            }

            return shipHangar;
        }
    }
}
