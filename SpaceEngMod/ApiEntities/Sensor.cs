using System;

using Sandbox.Common;
using Sandbox.Common.Components;
using Sandbox.Common.ObjectBuilders;
using Sandbox.ModAPI.Ingame;

using SPX.Station.Infrastructure.ApiEntities.Enums;
using SPX.Station.Infrastructure.Events;
using SPX.Station.Infrastructure.Utils;

namespace SPX.Station.Infrastructure.ApiEntities
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_SensorBlock))]
    public sealed class Sensor : EntityComponent<IMySensorBlock>
    {
        private string _hangarCode;

        public Sensor()
            : base("Sensor", "IMySensorBlock")
        {
            _sensorType = SensorType.AutoDoor;
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
            Entity.NeedsUpdate |= MyEntityUpdateEnum.EACH_10TH_FRAME;
        }

        protected override void Detach()
        {
            Entity.StateChanged -= OnStateChanged;
        }

        private void OnStateChanged(bool state)
        {
            switch (SensorType)
            {
                case SensorType.AutoDoor:
                    EntityEvents.AutoDoorSensorStateChanged.Raise(this, state);
                    break;
                case SensorType.HangarLights:
                    EntityEvents.HangarLightsSensorStateChanged.Raise(this, state);
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

        private SensorType _sensorType;
        public SensorType SensorType
        {
            get
            {

                _sensorType = SensorType.None;

                var options = new Options(Entity.CustomName);
                Enum.TryParse(options.Get("Type", string.Empty), true, out _sensorType);
                
                Log.Write("Sensor name {0} has SensorType: {1}", Entity.CustomName, _sensorType);

                return _sensorType;
            }
        }
    }
}