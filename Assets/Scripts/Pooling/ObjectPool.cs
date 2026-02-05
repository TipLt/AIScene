using System.Collections.Generic;
using UnityEngine;

namespace AIScene.Pooling
{
    /// <summary>
    /// Generic object pooling system for managing reusable game objects.
    /// Used for both player units and enemy units to optimize performance.
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        [Header("Pool Settings")]
        [SerializeField] private GameObject prefab;
        [SerializeField] private int initialPoolSize = 10;
        [SerializeField] private bool expandable = true;

        private Queue<GameObject> availableObjects = new Queue<GameObject>();
        private List<GameObject> allObjects = new List<GameObject>();
        private Transform poolContainer;

        private void Awake()
        {
            InitializePool();
        }

        /// <summary>
        /// Initialize the pool with the specified number of objects.
        /// </summary>
        private void InitializePool()
        {
            poolContainer = new GameObject($"{prefab.name}_Pool").transform;
            poolContainer.SetParent(transform);

            for (int i = 0; i < initialPoolSize; i++)
            {
                CreateNewObject();
            }
        }

        /// <summary>
        /// Create a new object and add it to the pool.
        /// </summary>
        private GameObject CreateNewObject()
        {
            GameObject obj = Instantiate(prefab, poolContainer);
            obj.SetActive(false);
            availableObjects.Enqueue(obj);
            allObjects.Add(obj);
            return obj;
        }

        /// <summary>
        /// Get an object from the pool. Creates a new one if pool is expandable and empty.
        /// </summary>
        /// <param name="position">Position to spawn the object</param>
        /// <param name="rotation">Rotation of the spawned object</param>
        /// <returns>The pooled object, or null if pool is empty and not expandable</returns>
        public GameObject GetObject(Vector3 position, Quaternion rotation)
        {
            if (availableObjects.Count == 0)
            {
                if (expandable)
                {
                    CreateNewObject();
                }
                else
                {
                    Debug.LogWarning($"Object pool for {prefab.name} is empty and not expandable.");
                    return null;
                }
            }

            GameObject obj = availableObjects.Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
            return obj;
        }

        /// <summary>
        /// Return an object to the pool.
        /// </summary>
        /// <param name="obj">The object to return</param>
        public void ReturnObject(GameObject obj)
        {
            if (obj == null) return;

            obj.SetActive(false);
            obj.transform.SetParent(poolContainer);
            
            if (!availableObjects.Contains(obj))
            {
                availableObjects.Enqueue(obj);
            }
        }

        /// <summary>
        /// Return all active objects to the pool.
        /// </summary>
        public void ReturnAllObjects()
        {
            foreach (var obj in allObjects)
            {
                if (obj != null && obj.activeSelf)
                {
                    ReturnObject(obj);
                }
            }
        }

        /// <summary>
        /// Get the count of available objects in the pool.
        /// </summary>
        public int AvailableCount => availableObjects.Count;

        /// <summary>
        /// Get the total count of objects in the pool.
        /// </summary>
        public int TotalCount => allObjects.Count;

        /// <summary>
        /// Get all active objects in the pool.
        /// </summary>
        public List<GameObject> GetActiveObjects()
        {
            List<GameObject> activeObjects = new List<GameObject>();
            foreach (var obj in allObjects)
            {
                if (obj != null && obj.activeSelf)
                {
                    activeObjects.Add(obj);
                }
            }
            return activeObjects;
        }
    }
}
