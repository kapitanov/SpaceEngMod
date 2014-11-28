using System.Collections.Generic;

using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;

using SPX.Station.Infrastructure.Utils;

namespace SPX.Station.Infrastructure.ApiEntities
{
    public static class Entities
    {
        public static bool FilterByName(IMyTerminalBlock entity)
        {
            if (entity.CustomName == null)
            {
                return false;
            }

            return entity.CustomName.StartsWith(Constants.SpacePortPrefix);
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

        public static List<AbstractPiston> Pistons = new List<AbstractPiston>();

        public static void Add(AbstractPiston entity)
        {
            Log.Scope("Piston added ({0})", entity.Entity.CustomName);
            Pistons.Add(entity);
        }

        public static void Remove(AbstractPiston entity)
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