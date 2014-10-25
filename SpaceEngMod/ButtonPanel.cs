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
    }
}