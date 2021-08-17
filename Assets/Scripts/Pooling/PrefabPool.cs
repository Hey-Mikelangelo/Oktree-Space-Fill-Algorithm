using System.Collections.Generic;
using UnityEngine;

namespace Quests.ObjectPool
{
    public class PrefabPool
    {
        private readonly GameObject _prototype;
        private readonly Queue<IPoolable> _readyElements;

        public PrefabPool(GameObject prototype, int initialInstances)
        {
            _prototype = prototype;
            _readyElements = new Queue<IPoolable>();

            for (int i = 0; i < initialInstances; i++)
            {
                var instance = CreateNewInstance();
                instance.PrepareForPooling();
                _readyElements.Enqueue(instance);
            }
        }

        public void AcceptReturning(IPoolable obj)
        {
            obj.PrepareForPooling();
            _readyElements.Enqueue(obj);
        }

        public IPoolable GetInstance()
        {
            var instance = _readyElements.Count < 1 ? CreateNewInstance() : _readyElements.Dequeue();
            instance.HandleSpawn();
            return instance;
        }

        private IPoolable CreateNewInstance()
        {
            var go = Object.Instantiate(_prototype);
            var poolable = go.GetComponent<IPoolable>();
            poolable.SetParentPool(this);
            return poolable;
        }

    }
}