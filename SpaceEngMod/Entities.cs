using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;

namespace SpaceEngMod
{
    public static class Entities
    {
        private const string Prefix = "D:";

        public static bool FilterByName(IMyTerminalBlock entity)
        {
            if (entity.CustomName == null)
            {
                return false;
            }

            return entity.CustomName.StartsWith(Prefix);
        }

        public static List<Sensor> Sensors = new List<Sensor>();

        public static void Add(Sensor entity)
        {
            Sensors.Add(entity);
        }

        public static void Remove(Sensor entity)
        {
            Sensors.Remove(entity);
        }

        public static List<Piston> Pistons = new List<Piston>();

        public static void Add(Piston entity)
        {
            Pistons.Add(entity);
        }

        public static void Remove(Piston entity)
        {
            Pistons.Remove(entity);
        }

        public static List<ButtonPanel> ButtonPanels = new List<ButtonPanel>();

        public static void Add(ButtonPanel entity)
        {
            ButtonPanels.Add(entity);
        }

        public static void Remove(ButtonPanel entity)
        {
            ButtonPanels.Remove(entity);
        }

        public static List<LandingGear> LandingGears = new List<LandingGear>();

        public static void Add(LandingGear entity)
        {
            LandingGears.Add(entity);
        }

        public static void Remove(LandingGear entity)
        {
            LandingGears.Remove(entity);
        }

        public static void PrintTerminalActions(string entityType, IMyTerminalBlock block)
        {
            return;
            var actions = new List<ITerminalAction>();
            block.GetActions(actions, _ => true);
            using (Log.Scope("{0}.GetActions:", block.BlockDefinition.TypeIdString))
            {
                foreach (var action in actions)
                {
                    Log.Write("Id = {0}, Name = {1}, Icon = {2}", action.Id, action.Name, action.Icon);
                }
            }
        }
    }
}