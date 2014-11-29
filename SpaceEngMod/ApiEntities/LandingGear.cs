using Sandbox.Common.Components;
using Sandbox.Common.ObjectBuilders;
using Sandbox.ModAPI.Ingame;

using SPX.Station.Infrastructure.Controllers;
using SPX.Station.Infrastructure.Controllers.Common;
using SPX.Station.Infrastructure.Events;
using SPX.Station.Infrastructure.Utils;

namespace SPX.Station.Infrastructure.ApiEntities
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_LandingGear))]
    public sealed class LandingGear : EntityComponent<IMyLandingGear>
    {
        private readonly TerminalAction _lockAction;
        private readonly TerminalAction _unlockAction;
        private readonly TerminalAction _toggleLockAction;

        private string _hangarCode;

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

        public string HangarCode
        {
            get
            {
                var options = new Options(Entity.CustomName);
                _hangarCode = options.Get("HC", string.Empty);

                return _hangarCode;
            }
        }
    }
}