using Sandbox.Common;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceEngMod
{
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
    public class GlobalCommandHandler : MySessionComponentBase
    {
        private bool _isInitialized;

        public override void UpdateBeforeSimulation()
        {
            if (!_isInitialized)
            {
                Initialize();
                _isInitialized = true;
            }

            base.UpdateBeforeSimulation();
        }

        private void Initialize()
        {
            using (Log.Scope("GlobalCommandHandler.Init()"))
            {
                try
                {
                    MyAPIGateway.Utilities.MessageEntered += OnMessageEntered;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        protected override void UnloadData()
        {
            if (!_isInitialized)
            {
                return;
            }

       //     using (Log.Scope("GlobalCommandHandler.UnloadData()"))
            {
                try
                {
                    if (!_isInitialized)
                    {
                        return;
                    }

                    MyAPIGateway.Utilities.MessageEntered -= OnMessageEntered;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        private void OnMessageEntered(string messageText, ref bool sendToOthers)
        {
            using (Log.Scope("GlobalCommandHandler.OnMessageEntered('{0})", messageText))
            {
                try
                {
                    messageText = messageText.ToLowerInvariant();

                    if (!messageText.StartsWith("spx_") || messageText.Length <= 4)
                    {
                        return;
                    }

                    sendToOthers = false;

                    messageText = messageText.Substring(4);
                    var parts = messageText.Split(' ');
                    var commandName = parts[0];
                    var commandArgs = CommandArgs.Parse(parts.Skip(1));
                    Log.Write("Command: name: '{0}', args: {1}", commandName, commandArgs);

                    switch (commandName)
                    {
                        case "aplist":
                            Log.Write("Command 'spx_aplist'");
                            ShipListCommandHandler(commandArgs);
                            break;

                        case "apstart":
                            Log.Write("Command 'spx_apstart'");
                            StartMovementCommandHandler(commandArgs);
                            break;


                        case "apstop":
                            Log.Write("Command 'spx_apstop'");
                            StopMovementCommandHandler(commandArgs);
                            break;

                        default:
                            Log.Write("Unknown command");
                            MyAPIGateway.Utilities.ShowNotification(string.Format("Unknown command: '{0}'", commandName), font: MyFontEnum.Red);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        private static void ShipListCommandHandler(CommandArgs args)
        {
            var name = args.GetString(0);
            var ships = GetShips(name);

            if (string.IsNullOrEmpty(name))
            {
                Log.Write("select ships -> [");
            }
            else
            {
                Log.Write("select ships where name like '{0}' -> [", name);
            }

            foreach (var ship in ships)
            {
                Log.Write("   '{0}' [{1}]", ship.GameObject.DisplayName, ship.GameObject.EntityId);
            }

            Log.Write("]");

            string message;
            if (ships.Length == 0)
            {
                message = "No ships found";
            }
            else if (ships.Length < 6)
            {
                message = string.Format(
                    "Got ship(s): {0}",
                    string.Join(", ", ships.Select(_ => _.GameObject.DisplayName))
                    );
            }
            else
            {
                message = string.Format("Got ship(s): {0}... ({1} more)",
                    string.Join(", ", ships.Take(5).Select(_ => _.GameObject.DisplayName)),
                    ships.Length - 5
                    );
            }

            MyAPIGateway.Utilities.ShowNotification(message);
        }

        private static void StartMovementCommandHandler(CommandArgs args)
        {
            var name = args.GetString(0);
            if (string.IsNullOrEmpty(name))
            {
                MyAPIGateway.Utilities.ShowNotification("Not enough arguments", font: MyFontEnum.Red);
                return;
            }

            var ships = GetShips(name);
            if (ships.Length == 0)
            {
                MyAPIGateway.Utilities.ShowNotification("No ships found", font: MyFontEnum.Red);
                return;
            }

            var destination = MyAPIGateway.Session.Player.GetPosition();
            
            foreach (var ship in ships)
            {
                ship.StartProgram(new FlightToProgram(destination));
            //    ship.StartProgram(new TrackPlayerProgram(MyAPIGateway.Session.Player));
            }
        }

        private static void StopMovementCommandHandler(CommandArgs args)
        {
            var name = args.GetString(0);
            if (string.IsNullOrEmpty(name))
            {
                MyAPIGateway.Utilities.ShowNotification("Not enough arguments", font: MyFontEnum.Red);
                return;
            }

            var ships = GetShips(name);
            if (ships.Length == 0)
            {
                MyAPIGateway.Utilities.ShowNotification("No ships found", font: MyFontEnum.Red);
                return;
            }

            foreach (var ship in ships)
            {
                ship.StopProgram();
            }
        }
        
        private static CubeGrid[] GetShips(string name)
        {
            var query = Entities.CubeGrids.Where(_ => !_.GameObject.IsStatic);
            if (name != null)
            {
                var list = new List<CubeGrid>();
                name = name.Trim().ToLowerInvariant();
                foreach (var ship in query)
                {
                    if (ship.GameObject.DisplayName.ToLowerInvariant().Contains(name))
                    {
                        list.Add(ship);
                    }
                }

                return list.ToArray();
            }

            return query.ToArray();
        }
    }
}
