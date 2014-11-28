using Sandbox.Common.Components;
using Sandbox.Common.ObjectBuilders;

namespace SPX.Station.Infrastructure.ApiEntities
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_ExtendedPistonBase))]
    public class ExtendedPistonBase : AbstractPiston
    {
    }
}