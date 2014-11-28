using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.Common;
using Sandbox.Common.Components;
using Sandbox.Common.ObjectBuilders;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using VRageMath;

namespace SpaceEngMod
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_CubeGrid))]
    public sealed class CubeGrid : MyGameLogicComponent
    {
        private sealed class FlightContext : IFlightContext
        {
            private readonly Value3D _position = new Value3D();

            private readonly Value1D _pitch = new Value1D();
            private readonly Value1D _yaw = new Value1D();
            private readonly Value1D _roll = new Value1D();

            private readonly Value1D _velocity = new Value1D();
            private readonly Value1D _acceleration = new Value1D();

            private readonly IMyCubeGrid _ship;
            private readonly IMyControllableEntity _controller;

            private float _dtime;
            private DateTime _lastTime;

            public FlightContext(IMyCubeGrid ship, IMyControllableEntity controller)
            {
                _ship = ship;
                _controller = controller;
                _lastTime = MyAPIGateway.Session.GameDateTime;
            }

            IMyCubeGrid IFlightContext.Ship { get { return _ship; } }
            IMyEntity IFlightContext.Entity { get { return (IMyEntity)_controller; } }
            IMyControllableEntity IFlightContext.Controller { get { return _controller; } }

            Value3D IFlightContext.Position { get { return _position; } }
            Value1D IFlightContext.Pitch { get { return _pitch; } }
            Value1D IFlightContext.Yaw { get { return _yaw; } }
            Value1D IFlightContext.Roll { get { return _roll; } }

            Value1D IFlightContext.Velocity { get { return _velocity; } }
            Value1D IFlightContext.Acceleration { get { return _acceleration; } }

            float IFlightContext.DeltaT { get { return _dtime; } }

            void IFlightContext.Log(string message)
            {
                Log.Write(string.Format("[{0}]: {1}", _ship.DisplayName, message));
            }

            public void Update()
            {
                var time = MyAPIGateway.Session.GameDateTime;
                _dtime = (float)(time - _lastTime).TotalSeconds;
                _lastTime = time;

                var pitch = (float)((IMyEntity)_controller).WorldMatrix.Up.Dot(Vector3.One);
                var yaw = (float)((IMyEntity)_controller).WorldMatrix.Left.Dot(Vector3.One);
                var roll = (float)((IMyEntity)_controller).WorldMatrix.Forward.Dot(Vector3.One);

                _position.Update(_ship.GetPosition(), _dtime);
                _pitch.Update(pitch, _dtime);
                _yaw.Update(yaw, _dtime);
                _roll.Update(roll, _dtime);
                _velocity.Update(_position.Differential.Length(), _dtime);
                _acceleration.Update(_velocity.Differential, _dtime);

                Log.Write(string.Format(
                    "[{0}]: pitch = {1:0.000}, yaw = {2:0.000}, roll = {3:0.000}, velocity = {4:0.0} m/s, acceleration = {5:0.0} m/s^2, dt = {6:0.0} ms",
                    _ship.DisplayName,
                    _pitch.Value,
                    _yaw.Value,
                    _roll.Value,
                    _velocity.Value,
                    _acceleration.Value,
                    _dtime * 1000
                    ));
            }
        }


        private IMyCubeGrid _cubeGrid;
        private IMyControllableEntity _controller;
        private FlightContext _context;
        private IFlightProgram _program;

        public IMyCubeGrid GameObject { get { return _cubeGrid; } }

        public override MyObjectBuilder_EntityBase GetObjectBuilder(bool copy = false)
        {
            return Entity.GetObjectBuilder(copy);
        }

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            using (Log.Scope("CubeGrid.Init()"))
            {
                try
                {
                    if (base.Entity == null)
                    {
                        Log.Write("Init() called but base.Entity is null");
                        return;
                    }

                    _cubeGrid = Entity as IMyCubeGrid;
                    if (_cubeGrid == null)
                    {
                        Log.Write("Init() called but base.Entity is not a IMyCubeGrid");
                        return;
                    }

                    Entities.Add(this);
                    Entity.NeedsUpdate |= MyEntityUpdateEnum.EACH_FRAME;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public override void UpdateBeforeSimulation()
        {
            if (_program == null)
            {
                return;
            }

            try
            {
                if (EnsureControllerIsInitialized())
                {
                    _context.Update();

                    var programTerminated = false;
                    _program.Update(_context, ref programTerminated);
                    if (programTerminated)
                    {
                        StopProgram();
                    }
                }
                else
                {
                    StopProgram();
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public void StartProgram(IFlightProgram program)
        {
            _program = program;
            _context.Update();

            _program.Initialize(_context);
            MyAPIGateway.Utilities.ShowMessage(_cubeGrid.DisplayName, string.Format("Program {0} started", _program.Name));
        }

        public void StopProgram()
        {
            if (_program != null)
            {
                MyAPIGateway.Utilities.ShowMessage(_cubeGrid.DisplayName, string.Format("Program {0} terminated", _program.Name));
            }

            _program = null;
            if (_controller != null)
            {
                _controller.MoveAndRotateStopped();
            }
        }

        private bool EnsureControllerIsInitialized()
        {
            if (_controller == null)
            {
                var blocks = new List<IMySlimBlock>();
                _cubeGrid.GetBlocks(blocks, _ => _.FatBlock is IMyControllableEntity);
                if (blocks.Count == 0)
                {
                    Log.Write("Ship '{0}' has no cockpit and will immediately stop", _cubeGrid.DisplayName);
                    return false;
                }

                foreach (var block in blocks)
                {
                    Log.Write(
                        "IMyControllableEntity: TypeId: {0}, SubtypeId: {1}, SubtypeName: {2}",
                        block.GetObjectBuilder().TypeId,
                        block.GetObjectBuilder().SubtypeId,
                        block.GetObjectBuilder().SubtypeName);
                }

                var cockpitBuilderType = MyObjectBuilderType.Parse("MyObjectBuilder_Cockpit");
                var controller = blocks.Select(_ => new
                {
                    Block = _.FatBlock as IMyControllableEntity,
                    Entity = _
                })
                    .FirstOrDefault(
                        _ => _.Block != null &&
                             _.Entity.GetObjectBuilder().TypeId == cockpitBuilderType);
                if (controller == null)
                {
                    Log.Write("Ship '{0}' has no REAL cockpit and will immediately stop", _cubeGrid.DisplayName);
                    return false;
                }

                if (controller.Entity == null)
                {
                    Log.Write(
                        "Ship '{0}' has cockpit '{1}'",
                        _cubeGrid.DisplayName,
                        controller);
                }
                else
                {
                    Log.Write(
                        "Ship '{0}' has cockpit '{1}' of type '{2}'/'{3}'",
                        _cubeGrid.DisplayName,
                        controller.Entity,
                        controller.Entity.GetObjectBuilder().TypeId,
                        controller.Entity.GetObjectBuilder().SubtypeId
                        );
                }

                _controller = controller.Block;
                _context = new FlightContext(_cubeGrid, _controller);
            }

            return true;
        }
    }

    public static class SMath
    {
        /*

                    var pitch = (float)((IMyEntity)_controller).WorldMatrix.Up.Dot(Vector3.One);
                    var yaw = (float)((IMyEntity)_controller).WorldMatrix.Left.Dot(Vector3.One);
                    var roll = (float)((IMyEntity)_controller).WorldMatrix.Forward.Dot(Vector3.One);
    */

        public static void ComputePitchYawRoll(
            ref MatrixD worldMatrix,
            ref Vector3 direction,
            out float pitch,
            out float yaw,
            out float roll
            )
        {
            pitch = (float)(worldMatrix.Up.Dot(Vector3.One));
            yaw = (float)(worldMatrix.Left.Dot(Vector3.One));
            roll = (float)(worldMatrix.Forward.Dot(Vector3.One));
        }
    }

}
