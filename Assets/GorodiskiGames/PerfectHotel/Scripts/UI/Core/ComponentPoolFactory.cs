using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.UI.Pool
{
    public class ComponentPoolFactory : MonoBehaviour, IComponentPoolFactory
    {
        [SerializeField]
        private List<GameObject> _prefabs = new List<GameObject>();
        [SerializeField, HideInInspector, FormerlySerializedAs("_prefab")]
        private GameObject _legacyPrefab;
        [SerializeField]
        private int _count;
        [SerializeField]
        private Transform _content;
        [SerializeField]
        private Transform _poolStorage;

        private readonly HashSet<GameObject> _instances;
        private readonly Dictionary<GameObject, GameObject> _instancePrefabs;
        private List<GameObject> _pool;

        public Transform Content { get { return _content; } }

        public ComponentPoolFactory()
        {
            _instances = new HashSet<GameObject>();
            _instancePrefabs = new Dictionary<GameObject, GameObject>();
            _pool = new List<GameObject>();
        }

        public int CountInstances
        {
            get { return _instances.Count; }
        }

        private void Awake()
        {
            MigrateLegacyPrefabIfNeeded();

            if (_instances.Count > 0)
                return;

            for (int i = 0; i < _count; i++)
            {
                Get<Transform>();
            }
            ReleaseAllInstances();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            MigrateLegacyPrefabIfNeeded();
        }
#endif

        public T Get<T>() where T : Component
        {
            return Get<T>(_instances.Count);
        }

        public T Get<T>(int sublingIndex) where T : Component
        {
            bool isNewInstance = false;
            GameObject selectedPrefab = GetRandomPrefab<T>();
            if (selectedPrefab == null)
            {
                Debug.LogWarningFormat(this,
                    "{0} on {1} has no prefabs configured for component {2}.",
                    nameof(ComponentPoolFactory), name, typeof(T).Name);
                return null;
            }

            GameObject pooledObject;
            if (!TryTakePooledObject(selectedPrefab, out pooledObject))
            {
                pooledObject = CreateInstance(selectedPrefab);
                isNewInstance = true;
            }

            T resultComponent = pooledObject.GetComponent<T>();
            if (null == resultComponent)
            {
                return resultComponent;
            }

            var go = resultComponent.gameObject;
            var t = resultComponent.transform;
            if (isNewInstance || (_poolStorage != null && _poolStorage != _content))
            {
                t.SetParent(_content, false);
            }

            _instances.Add(go);

            if (!go.activeSelf)
            {
                go.SetActive(true);
            }

            if (t.GetSiblingIndex() != sublingIndex)
            {
                t.SetSiblingIndex(sublingIndex);
            }

            return resultComponent;
        }

        public void Release<T>(T component) where T : Component
        {
            var go = component.gameObject;
            if (_instances.Contains(go))
            {
                go.SetActive(false);
                if (_poolStorage)
                {
                    go.transform.SetParent(_poolStorage, false);
                }
                _pool.Add(go);
                _instances.Remove(go);
            }
        }

        public void ReleaseAllInstances()
        {
            foreach (GameObject instance in _instances)
            {
                instance.SetActive(false);
                if (_poolStorage)
                {
                    instance.transform.SetParent(_poolStorage, false);
                }
                _pool.Add(instance);
            }
            _instances.Clear();
        }

        public void PutInstancesToPool()
        {
            _pool = new List<GameObject>(_instances.Union(_pool));
            _instances.Clear();
        }

        public void HideUnusedInstances()
        {
            foreach (GameObject instance in _pool)
            {
                instance.SetActive(false);
            }
        }

        public void Dispose()
        {
            ReleaseAllInstances();

            foreach (GameObject gameObject in _pool)
            {
                GameObject.Destroy(gameObject);
            }
            _pool.Clear();
        }

        private GameObject CreateInstance(GameObject prefab)
        {
            GameObject instance = Instantiate(prefab);
            _instancePrefabs[instance] = prefab;
            return instance;
        }

        private GameObject GetRandomPrefab<T>() where T : Component
        {
            MigrateLegacyPrefabIfNeeded();

            if (_prefabs == null || _prefabs.Count == 0)
            {
                return null;
            }

            int compatiblePrefabsCount = 0;
            for (int i = 0; i < _prefabs.Count; i++)
            {
                GameObject prefab = _prefabs[i];
                if (prefab != null && prefab.GetComponent<T>() != null)
                {
                    compatiblePrefabsCount++;
                }
            }

            if (compatiblePrefabsCount == 0)
            {
                return null;
            }

            int randomIndex = Random.Range(0, compatiblePrefabsCount);

            for (int i = 0; i < _prefabs.Count; i++)
            {
                GameObject prefab = _prefabs[i];
                if (prefab == null || prefab.GetComponent<T>() == null)
                {
                    continue;
                }

                if (randomIndex == 0)
                {
                    return prefab;
                }

                randomIndex--;
            }

            return null;
        }

        private bool TryTakePooledObject(GameObject selectedPrefab, out GameObject pooledObject)
        {
            for (int i = 0; i < _pool.Count; i++)
            {
                GameObject current = _pool[i];
                if (current == null)
                {
                    continue;
                }

                GameObject prefab;
                if (!_instancePrefabs.TryGetValue(current, out prefab) || prefab != selectedPrefab)
                {
                    continue;
                }

                pooledObject = current;
                _pool.RemoveAt(i);
                return true;
            }

            pooledObject = null;
            return false;
        }

        private void MigrateLegacyPrefabIfNeeded()
        {
            if (_legacyPrefab == null)
            {
                return;
            }

            if (_prefabs == null)
            {
                _prefabs = new List<GameObject>();
            }

            if (_prefabs.Contains(_legacyPrefab))
            {
                return;
            }

            _prefabs.Add(_legacyPrefab);
        }
    }
}
