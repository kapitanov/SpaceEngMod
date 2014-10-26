using System;
using System.Collections.Generic;

namespace SpaceEngMod
{
    public sealed class EntityEvent<TEntity>
    {
        private readonly List<Action<TEntity>> _handlers = new List<Action<TEntity>>();

        public void Subscribe(Action<TEntity> handler)
        {
            _handlers.Add(handler);
        }

        public void Raise(TEntity entity)
        {
            foreach (var handler in _handlers)
            {
                handler(entity);
            }
        }
    }

    public sealed class EntityEvent<TEntity, TArg>
    {
        private readonly List<Action<TEntity, TArg>> _handlers = new List<Action<TEntity, TArg>>();

        public void Subscribe(Action<TEntity, TArg> handler)
        {
            _handlers.Add(handler);
        }

        public void Raise(TEntity entity, TArg arg)
        {
            foreach (var handler in _handlers)
            {
                handler(entity, arg);
            }
        }
    }
}