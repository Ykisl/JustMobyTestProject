using Game.GamePool;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GamePool
{
    public class BaseGamePool : IGamePool
    {
        private GameObject _rootGameObject;

        private bool _isInitialized;
        private GameObject _prefab;
        private bool _isAllowToCreateNewItems;

        private List<GameObject> _items;
        private List<GameObject> _activeItems;
        private List<GameObject> _inactiveItems;

        public event Action<IGamePool> OnDispose;

        public BaseGamePool(GameObject rootGameObject)
        {
            _rootGameObject = rootGameObject;

            _items = new List<GameObject>();
            _activeItems = new List<GameObject>();
            _inactiveItems = new List<GameObject>();
        }

        public virtual void Initialize(GameObject prefab, int startCount = 5, bool isAllowToCreateNewItems = true)
        {
            _prefab = prefab;
            _isAllowToCreateNewItems = isAllowToCreateNewItems;

            _isInitialized = true;

            for (int i = 0; i <= startCount; i++)
            {
                CreateNewItem();
            }
        }

        public virtual GameObject Take()
        {
            if (_inactiveItems.Count <= 0)
            {
                if (!_isAllowToCreateNewItems)
                {
                    return null;
                }

                if (CreateNewItem() == null)
                {
                    return null;
                }
            }

            var newItem = _inactiveItems[0];
            _inactiveItems.Remove(newItem);
            _activeItems.Add(newItem);

            return newItem;

        }

        public virtual void Recycle(GameObject item)
        {
            if (!_activeItems.Contains(item))
            {
                return;
            }

            RecycleInternal(item);
            _activeItems.Remove(item);
            _inactiveItems.Add(item);
        }

        public virtual void RecycleAll()
        {
            foreach (var activeItem in _activeItems)
            {
                RecycleInternal(activeItem);
            }

            _inactiveItems.AddRange(_activeItems);
            _activeItems.Clear();
        }

        public virtual void ClearAll()
        {
            foreach (var item in _items)
            {
                GameObject.Destroy(item);
            }

            _items.Clear();
            _activeItems.Clear();
            _inactiveItems.Clear();
        }

        public virtual void Dispose()
        {
            if (!_isInitialized)
            {
                return;
            }

            ClearAll();

            OnDispose?.Invoke(this);
        }

        protected virtual GameObject CreateNewItem()
        {
            if (_prefab == null)
            {
                return null;
            }

            var newGameObject = GameObject.Instantiate(_prefab, Vector3.zero, Quaternion.identity, _rootGameObject.transform);
            newGameObject.SetActive(false);

            _items.Add(newGameObject);
            _inactiveItems.Add(newGameObject);

            return newGameObject;

        }

        protected virtual void RecycleInternal(GameObject item)
        {
            item.gameObject.SetActive(false);

            item.transform.position = Vector3.zero;
            item.transform.rotation = Quaternion.identity;
        }
    }
}

