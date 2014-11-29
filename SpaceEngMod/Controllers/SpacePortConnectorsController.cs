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
    public sealed class SpacePortConnectorsController : MySessionComponentBase
    {
        private bool _initialized;

        public override void UpdateBeforeSimulation()
        {
            if (!_initialized)
            {
                using (Log.Scope("SpacePortGearsController.Initialize Version 1.4 \r\n"))
                {
                    EntityEvents.ConnectorsButtonPressed.Subscribe(EntityEvents_ButtonPressed);
                    EntityEvents.ConnectorsButtonUpdate100.Subscribe(EntityEvents_ButtonUpdate100);
                    _initialized = true;
                }
            }

            base.UpdateBeforeSimulation();
        }

        private void EntityEvents_ButtonPressed(ButtonPanel buttonPanel, int button)
        {
            try
            {
                var hangarConnectors = GetHangarConnectors(buttonPanel);
                if (hangarConnectors != null)
                {
                    hangarConnectors.Toggle(this);
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
                    var autodoor = GetHangarConnectors(buttonPanel);
                    if (autodoor != null)
                    {
                        if (!_notificationShown)
                        {
                            MyAPIGateway.Utilities.ShowNotification("This button controls hangar connectors", 1000, MyFontEnum.Green);
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

        private readonly Dictionary<ButtonPanel, ShipHangarConnectors> _hangarConnectors = new Dictionary<ButtonPanel, ShipHangarConnectors>();

        private ShipHangarConnectors GetHangarConnectors(ButtonPanel button)
        {
            Log.Write("GetHangarConnectors initiated {0}", button.Entity.CustomName);
            if (!button.Entity.CustomName.StartsWith(Constants.SpacePortPrefix, StringComparison.OrdinalIgnoreCase) || button.ButtonType != ButtonType.Connectors)
            {
                return null;
            }

            ShipHangarConnectors shipHangar;
            if (!_hangarConnectors.TryGetValue(button, out shipHangar))
            {
                shipHangar = new ShipHangarConnectors(button);
                _hangarConnectors.Add(button, shipHangar);
            }

            return shipHangar;
        }
    }
}