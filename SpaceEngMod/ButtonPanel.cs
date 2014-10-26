using Sandbox.Common;
using Sandbox.Common.Components;
using Sandbox.Common.ObjectBuilders;
using Sandbox.ModAPI.Ingame;

namespace SpaceEngMod
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_ButtonPanel))]
    public sealed class ButtonPanel : EntityComponent<IMyButtonPanel>
    {
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
            Log.Scope("ButtonPanel.OnButtonPressed({0})", key);
            EntityEvents.ButtonPressed.Raise(this, key);
        }

        public override void UpdateBeforeSimulation100()
        {
            EntityEvents.ButtonUpdate100.Raise(this);
        }
    }
}