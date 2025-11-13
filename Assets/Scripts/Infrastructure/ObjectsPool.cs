using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Infrastructure
{
    public class ObjectsPool<T> where T : Component
    {
        private readonly List<T> _objectsPool = new List<T>();
        private readonly T _prefab;
        private Transform _parent;

        public ObjectsPool(T prefab, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
        }

        public T GetItem()
        {
            foreach (var obj in _objectsPool)
            {
                if (!obj.gameObject.activeSelf)
                {
                    obj.gameObject.SetActive(true);
                    return obj;
                }
            }

            var instance = Object.Instantiate(_prefab, _parent);

            _objectsPool.Add(instance);
            return instance;
        }

        public void ReturnAll()
        {
            foreach (var obj in _objectsPool)
            {
                obj.gameObject.SetActive(false);
            }
        }

        public void SetParent(Transform parent)
        {
            _parent = parent;
        }
        
        public void ClearAll()
        {
            _objectsPool.Clear();
        }
    }
}