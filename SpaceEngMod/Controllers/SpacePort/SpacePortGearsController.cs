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
    public sealed class SpacePortGearsController : MySessionComponentBase
    {
        private bool _initialized;

        public override void UpdateBeforeSimulation()
        {
            if (!_initialized)
            {
                using (Log.Scope("SpacePortGearsController.Initialize Version 1.4 \r\n"))
                {
                    EntityEvents.GearsButtonPressed.Subscribe(EntityEvents_ButtonPressed);
                    EntityEvents.GearsButtonUpdate100.Subscribe(EntityEvents_ButtonUpdate100);
                    _initialized = true;
                }
            }

            base.UpdateBeforeSimulation();
        }

        private void EntityEvents_ButtonPressed(ButtonPanel buttonPanel, int button)
        {
            try
            {
                var hangarGears = GetHangarGears(buttonPanel);
                if (hangarGears != null)
                {
                    hangarGears.Toggle(this);
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
                    var autodoor = GetHangarGears(buttonPanel);
                    if (autodoor != null)
                    {
                        if (!_notificationShown)
                        {
                            MyAPIGateway.Utilities.ShowNotification("This button controls hangar landing gears", 1000, MyFontEnum.Green);
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

        private readonly Dictionary<ButtonPanel, ShipHangarGears> _hangarGears = new Dictionary<ButtonPanel, ShipHangarGears>();

        private ShipHangarGears GetHangarGears(ButtonPanel button)
        {
            Log.Write("GetHangarGears initiated {0}", button.Entity.CustomName);
            if (!button.Entity.CustomName.StartsWith(Constants.SpacePortPrefix, StringComparison.OrdinalIgnoreCase) || button.ButtonType != ButtonType.Gears)
            {
                return null;
            }

            ShipHangarGears shipHangar;
            if (!_hangarGears.TryGetValue(button, out shipHangar))
            {
                shipHangar = new ShipHangarGears(button);
                _hangarGears.Add(button, shipHangar);
            }

            return shipHangar;
        }
    }
}