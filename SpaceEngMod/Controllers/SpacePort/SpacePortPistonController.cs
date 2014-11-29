using System;
using System.Collections.Generic;

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
                var depot = GetShipHangar(buttonPanel);
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
                if ((MyAPIGateway.Session.Player.GetPosition() - buttonPanel.Entity.GetPosition()).Length() <= 2)
                {
                    var autodoor = GetShipHangar(buttonPanel);
                    if (autodoor != null)
                    {
                        if (!_notificationShown)
                        {
                            MyAPIGateway.Utilities.ShowNotification("This button controls hangar door", 1000, MyFontEnum.Green);
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

        private readonly Dictionary<ButtonPanel, ShipHangar> _hangarPistons = new Dictionary<ButtonPanel, ShipHangar>();

        private ShipHangar GetShipHangar(ButtonPanel button)
        {
            Log.Write("GetShipHangar initiated {0}", button.Entity.CustomName);
            if (!button.Entity.CustomName.StartsWith(Constants.SpacePortPrefix, StringComparison.OrdinalIgnoreCase) || button.ButtonType != ButtonType.Pistons)
            {
                return null;
            }

            ShipHangar shipHangar;
            if (!_hangarPistons.TryGetValue(button, out shipHangar))
            {
                shipHangar = new ShipHangar(button);
                _hangarPistons.Add(button, shipHangar);
            }

            return shipHangar;
        }
    }
}
