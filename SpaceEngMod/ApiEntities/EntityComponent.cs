﻿using System;

using Sandbox.Common.Components;
using Sandbox.Common.ObjectBuilders;
using Sandbox.ModAPI.Ingame;

using SPX.Station.Infrastructure.Controllers;
using SPX.Station.Infrastructure.Controllers.Common;
using SPX.Station.Infrastructure.Utils;

namespace SPX.Station.Infrastructure.ApiEntities
{
    public abstract class EntityComponent<T> : MyGameLogicComponent, IEntityController
        where T : class, IMyTerminalBlock
    {
        private readonly string _name;
        private readonly string _typename;

        private readonly TerminalAction _toggleAction;
        private readonly TerminalAction _onAction;
        private readonly TerminalAction _offAction;

        private T _typedEntity;
        private bool _attached;

        protected EntityComponent(string name, string typename)
        {
            _name = name;
            _typename = typename;

            _toggleAction = new TerminalAction(this, TerminalAction.OnOff);
            _onAction = new TerminalAction(this, TerminalAction.OnOff_On);
            _offAction = new TerminalAction(this, TerminalAction.OnOff_Off);
        }

        public new T Entity { get { return _typedEntity; } }

        IMyTerminalBlock IEntityController.Entity { get { return _typedEntity; } }
        string IEntityController.EntityName { get { return _typedEntity.CustomName; } }
        string IEntityController.EntityTypeName { get { return _typename; } }
        bool IEntityController.HasEntity { get { return _typedEntity != null; } }

        public void ToggleOnOff()
        {
            _toggleAction.Apply();
        }

        public void TurnOn()
        {
            _onAction.Apply();
        }

        public void TurnOff()
        {
            _offAction.Apply();
        }

        public override MyObjectBuilder_EntityBase GetObjectBuilder(bool copy = false)
        {
            return Entity.GetObjectBuilder(copy);
        }

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            using (Log.Scope("{0}.Init()", _name))
            {
                try
                {
                    if (base.Entity == null)
                    {
                        Log.Write("Init() called but base.Entity is null");
                        return;
                    }

                    _typedEntity = base.Entity as T;
                    if (_typedEntity == null)
                    {
                        Log.Write("Init() called but base.Entity is not a {0}", _typename);
                        return;
                    }
                    
                    Entities.PrintTerminalActions(_typename, _typedEntity);
                    _typedEntity.CustomNameChanged += OnCustomNameChanged;
                    
                    //Log.Write("Init() called on {0} with {1} {2} {3} {4}", 
                    //    _typename,
                    //    _typedEntity.Name,
                    //    _typedEntity.CustomName,
                    //    _typedEntity.DisplayName,
                    //    _typedEntity.GetFriendlyName()
                    //    );

                    //if (Entities.FilterByName(_typedEntity))
                    //{
                        AttachToEntity();
                    //}
                    //else
                    //{
                    //    Log.Write("Init() called but {0} with CustomName = {1} is not attachable", _typename, _typedEntity.CustomName);
                    //}
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        private void AttachToEntity()
        {
            if (!_attached)
            {
                Log.Write("Attaching to {0} with CustomName = {0}", _typename, _typedEntity.CustomName);
                OnCreated();
                Attach();

                _attached = true;
            }
        }

        private void DetachFromEntity()
        {
            if (_attached)
            {
                Log.Write("Detaching from {0} with CustomName = {1}", _typename, _typedEntity.CustomName);
                Detach();
                OnDestroyed();

                _attached = false;
            }
        }

        protected abstract void OnCreated();
        protected abstract void OnDestroyed();

        protected abstract void Attach();
        protected abstract void Detach();

        protected virtual void OnCustomNameChanged(IMyTerminalBlock obj)
        {
            //if (Entities.FilterByName(_typedEntity))
            //{
            //    AttachToEntity();
            //}
            //else
            //{
            //    DetachFromEntity();
            //}
        }

        public override void Close()
        {
            //using (Log.Scope("{0}.ToggleGreen()", _name))
            //{
            //    try
            //    {
            //        DetachFromEntity();
            //        _typedEntity.CustomNameChanged -= OnCustomNameChanged;

            //        OnDestroyed();
            //    }
            //    catch (Exception e)
            //    {
            //        Log.Error(e);
            //    }
            //}
        }
    }
}