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
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_InteriorLight))]
    public sealed class InteriorLight : EntityComponent<IMyFunctionalBlock>
    {
        public InteriorLight() : base("InteriorLight", "IMyFunctionalBlock") { }

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
            
        }

        protected override void Detach()
        {
            
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


        private SignalType _type;

        private string _hangarCode;

        public SignalType SignalType
        {
            get
            {

                _type = SignalType.None;

                var options = new Options(Entity.CustomName);
                Enum.TryParse(options.Get("Type", string.Empty), true, out _type);


                return _type;
            }
        }
    }
}