using Sandbox.Common;
using Sandbox.Common.Components;
using Sandbox.Common.ObjectBuilders;
using Sandbox.ModAPI.Ingame;

namespace SpaceEngMod
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_SensorBlock))]
    public sealed class Sensor : EntityComponent<IMySensorBlock>
    {
        public Sensor() : base("Sensor", "IMySensorBlock") { }

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
            Entity.NeedsUpdate |= MyEntityUpdateEnum.EACH_10TH_FRAME;
        }

        protected override void Detach()
        {
            Entity.StateChanged -= OnStateChanged;
        }

        private void OnStateChanged(bool state)
        {
            EntityEvents.SensorStateChanged.Raise(this, state);
        }
    }
}