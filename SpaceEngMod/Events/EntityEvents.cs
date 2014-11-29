using SPX.Station.Infrastructure.ApiEntities;

namespace SPX.Station.Infrastructure.Events
{
    public static class EntityEvents
    {
        public static readonly EntityEvent<Sensor, bool> AutoDoorSensorStateChanged = new EntityEvent<Sensor, bool>();
        public static readonly EntityEvent<Sensor, bool> HangarLightsSensorStateChanged = new EntityEvent<Sensor, bool>();

        public static readonly EntityEvent<AbstractPiston, bool> PistonLimitReached = new EntityEvent<AbstractPiston, bool>();

        public static readonly EntityEvent<LandingGear, bool> LandingGearStateChanged = new EntityEvent<LandingGear, bool>();

        public static readonly EntityEvent<Connector, bool> ConnectorStateChanged = new EntityEvent<Connector, bool>();

        public static readonly EntityEvent<ButtonPanel, int> PistonsButtonPressed =new EntityEvent<ButtonPanel, int>();
        public static readonly EntityEvent<ButtonPanel, int> GearsButtonPressed = new EntityEvent<ButtonPanel, int>();
        public static readonly EntityEvent<ButtonPanel, int> ConnectorsButtonPressed = new EntityEvent<ButtonPanel, int>();
        public static readonly EntityEvent<ButtonPanel, int> AutoDoorButtonPressed = new EntityEvent<ButtonPanel, int>();

        public static readonly EntityEvent<ButtonPanel> PistonsButtonUpdate100 = new EntityEvent<ButtonPanel>();
        public static readonly EntityEvent<ButtonPanel> GearsButtonUpdate100 = new EntityEvent<ButtonPanel>();
        public static readonly EntityEvent<ButtonPanel> ConnectorsButtonUpdate100 = new EntityEvent<ButtonPanel>(); 
        public static readonly EntityEvent<ButtonPanel> AutoDoorButtonUpdate100 = new EntityEvent<ButtonPanel>();
    }
    
}