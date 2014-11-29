using System;

using Sandbox.ModAPI.Ingame;

using SPX.Station.Infrastructure.Controllers;
using SPX.Station.Infrastructure.Controllers.Common;
using SPX.Station.Infrastructure.Events;
using SPX.Station.Infrastructure.Utils;

namespace SPX.Station.Infrastructure.ApiEntities
{
    public abstract class AbstractPiston : EntityComponent<IMyPistonBase>
    {
        private readonly TerminalAction _resetVelocityAction;
        private readonly TerminalAction _increaseVelocityAction;
        private readonly TerminalAction _decreaseVelocityAction;
        private readonly TerminalAction _reverseAction;

        private string _hangarCode;

        public AbstractPiston()
            : base("Piston", "IMyPistonBase")
        {
            _resetVelocityAction = new TerminalAction(this, TerminalAction.ResetVelocity);
            _increaseVelocityAction = new TerminalAction(this, TerminalAction.IncreaseVelocity);
            _decreaseVelocityAction = new TerminalAction(this, TerminalAction.DecreaseVelocity);
            _reverseAction = new TerminalAction(this, TerminalAction.Reverse);
        }

        public void SetVelocity(float velocity)
        {
            ResetVelocity(); // V = -0.5

            var velocityAbs = velocity > 0 ? velocity + 0.5 : -velocity;
            var count = (int)Math.Round(velocityAbs / 0.5);

            for (var i = 0; i < count; i++)
            {
                DecreaseVelocity(); // V = V - 0.5
            }

            if (velocity > 0)
            {
                Reverse(); // V = -V
            }
        }

        public void ResetVelocity()
        {
            _resetVelocityAction.Apply();
        }

        public void IncreaseVelocity()
        {
            _increaseVelocityAction.Apply();
        }

        public void DecreaseVelocity()
        {
            _decreaseVelocityAction.Apply();
        }

        public void Reverse()
        {
            _reverseAction.Apply();
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
            Entity.LimitReached += OnLimitReached;
        }

        protected override void Detach()
        {
            Entity.LimitReached -= OnLimitReached;
        }

        private void OnLimitReached(bool state)
        {
            EntityEvents.PistonLimitReached.Raise(this, state);
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