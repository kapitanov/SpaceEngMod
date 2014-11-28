using Sandbox.Common;

namespace SpaceEngMod
{
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
    public class LogInitializer : MySessionComponentBase
    {
        public override void LoadData()
        {
            Log.Initialize();
        }

        protected override void UnloadData()
        {
            Log.Release();
        }

    }
}