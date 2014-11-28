using System;

using Sandbox.Common;
using Sandbox.Common.Components;
using Sandbox.Common.ObjectBuilders;
using Sandbox.ModAPI.Ingame;

using SPX.Station.Infrastructure.Events;
using SPX.Station.Infrastructure.Utils;

namespace SPX.Station.Infrastructure.ApiEntities
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_ButtonPanel))]
    public sealed class ButtonPanel : EntityComponent<IMyButtonPanel>
    {
        private string _hangarCode;

        public ButtonPanel()
            : base("ButtonPanel", "IMyButtonPanel")
        { }

        protected override void OnCreated()
        {
            Entities.Add(this);
            Entity.NeedsUpdate |= MyEntityUpdateEnum.EACH_100TH_FRAME;
        }

        protected override void OnDestroyed()
        {
            Entities.Remove(this);
        }

        protected override void Attach()
        {
            Entity.ButtonPressed += OnButtonPressed;
        }

        protected override void Detach()
        {
            Entity.ButtonPressed -= OnButtonPressed;
        }
        
        private void OnButtonPressed(int key)
        {
            switch (ButtonType)
            {
                case ButtonType.Pistons:
                    EntityEvents.PistonsButtonPressed.Raise(this, key);
                    break;
                case ButtonType.Gears:
                    EntityEvents.GearsButtonPressed.Raise(this, key);
                    break;
                case ButtonType.Connectors:
                    EntityEvents.ConnectorsButtonPressed.Raise(this, key);
                    break;
                case ButtonType.AutoDoor:
                    EntityEvents.AutoDoorButtonPressed.Raise(this, key);
                    break;
            }
        }

        public override void UpdateBeforeSimulation100()
        {
            switch (ButtonType)
            {
                case ButtonType.Pistons:
                    EntityEvents.PistonsButtonUpdate100.Raise(this);
                    break;
                case ButtonType.Gears:
                    EntityEvents.GearsButtonUpdate100.Raise(this);
                    break;
                case ButtonType.Connectors:
                    EntityEvents.ConnectorsButtonUpdate100.Raise(this);
                    break;
                case ButtonType.AutoDoor:
                    EntityEvents.AutoDoorButtonUpdate100.Raise(this);
                    break;
            }
        }

        public string HangarCode
        {
            get
            {
               
                var options = new Options(Entity.CustomName);
                _hangarCode = options.Get("HC", string.Empty);

                return _hangarCode;
            }
        }


        
        private ButtonType _buttonType;
        public ButtonType ButtonType
        {
            get
            {
               
                _buttonType = ButtonType.None;

                var options = new Options(Entity.CustomName);
                Enum.TryParse(options.Get("Type", string.Empty), true, out _buttonType);
                
                
                return _buttonType;
            }
        }
    }
}