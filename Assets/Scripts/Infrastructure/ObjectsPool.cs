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
    }

    public class ObjectsPoolsFactory : MonoBehaviour
    {
        private Dictionary<Type, object> _pools = new();

        private static ObjectsPoolsFactory _instance;

        public static ObjectsPoolsFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("ObjectsPoolsFactory");
                    _instance = go.AddComponent<ObjectsPoolsFactory>();
                }

                return _instance;
            }
        }

        public ObjectsPool<T> GetPool<T>(T prefab) where T : Component
        {
            var key = typeof(T);
            if (!_pools.ContainsKey(key))
            {
                _pools.Add(key, new ObjectsPool<T>(prefab, transform));
            }

            return (ObjectsPool<T>)_pools[key];
        }

        private void OnDestroy()
        {
            _pools = null;
            _instance = null;
        }
    }
}