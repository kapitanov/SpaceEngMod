using System.Collections.Generic;

using Sandbox.Common.Components;
using Sandbox.Common.ObjectBuilders;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;

using SPX.Station.Infrastructure.Controllers;
using SPX.Station.Infrastructure.Controllers.Common;
using SPX.Station.Infrastructure.Events;
using SPX.Station.Infrastructure.Utils;

using IMyCubeBlock = Sandbox.ModAPI.IMyCubeBlock;

namespace SPX.Station.Infrastructure.ApiEntities
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_ConveyorConnector))]
    public sealed class Connector : EntityComponent<IMyTerminalBlock>
    {
        private readonly TerminalAction _lockAction;
        private readonly TerminalAction _unlockAction;
        private readonly TerminalAction _toggleLockAction;

        private string _hangarCode;

        public Connector()
            : base("Connector", "IMyTerminalBlock")
        {
            _lockAction = new TerminalAction(this, TerminalAction.Lock);
            _unlockAction = new TerminalAction(this, TerminalAction.Unlock);
            _toggleLockAction = new TerminalAction(this, TerminalAction.SwitchLock);


            using (Log.Scope("Connector Settins"))
            {
                List<ITerminalAction> resultList = new List<ITerminalAction>();
                MyAPIGateway.TerminalActionsHelper.GetActions(Entity.GetType(), resultList);


                foreach (var terminalAction in resultList)
                {
                    Log.Write("Id = {0}, Name = {1}, Icon = {2}", terminalAction.Id, terminalAction.Name, terminalAction.Icon);
                }
            }
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
            //Entity.StateChanged += OnStateChanged;
        }

        protected override void Detach()
        {
            //Entity.StateChanged -= OnStateChanged;
        }

        private void OnStateChanged(bool state)
        {
            EntityEvents.ConnectorStateChanged.Raise(this, state);
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