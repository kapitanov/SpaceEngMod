namespace SpaceEngMod
{
    public static class EntityEvents
    {
        public static readonly EntityEvent<Sensor, bool> SensorStateChanged = new EntityEvent<Sensor, bool>();

        public static readonly EntityEvent<Piston, bool> PistonLimitReached = new EntityEvent<Piston, bool>();

        public static readonly EntityEvent<LandingGear, bool> LandingGearStateChanged = new EntityEvent<LandingGear, bool>();

        public static readonly EntityEvent<ButtonPanel, int> ButtonPressed =new EntityEvent<ButtonPanel, int>();

        public static readonly EntityEvent<ButtonPanel> ButtonUpdate100 = new EntityEvent<ButtonPanel>();
    }
}