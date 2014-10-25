using Sandbox.Common.Components;
using Sandbox.Common.ObjectBuilders;
using Sandbox.ModAPI.Ingame;

namespace SpaceEngMod
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_LandingGear))]
    public sealed class LandingGear : EntityComponent<IMyLandingGear>
    {
        private readonly TerminalAction _lockAction;
        private readonly TerminalAction _unlockAction;
        private readonly TerminalAction _toggleLockAction;

        public LandingGear()
            : base("LandingGear", "IMyLandingGear")
        {
            _lockAction = new TerminalAction(this, TerminalAction.Lock);
            _unlockAction = new TerminalAction(this, TerminalAction.Unlock);
            _toggleLockAction = new TerminalAction(this, TerminalAction.SwitchLock);
        }
        
        public void Lock()
        {
            _lockAction.Apply();
        }

        public void Unlock()
        {
            _unlockAction.Apply();
        }

        public void ToggleLock()
        {
            _toggleLockAction.Apply();
        }
        
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
            Entity.StateChanged += OnStateChanged;
        }

        protected override void Detach()
        {
            Entity.StateChanged -= OnStateChanged;
        }

        private void OnStateChanged(bool state)
        {
            EntityEvents.LandingGearStateChanged.Raise(this, state);
        }
    }
}