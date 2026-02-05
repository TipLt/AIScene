using UnityEngine;
using AIScene.Pooling;
using System.Collections.Generic;

namespace AIScene.Management
{
    /// <summary>
    /// Manages enemy units including spawning and pooling.
    /// Coordinates with EnemyAI for player detection and chasing.
    /// </summary>
    public class EnemyManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ObjectPool enemyPool;
        [SerializeField] private Transform enemyParent;

        [Header("Spawn Settings")]
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private float groupSpacing = 1f;

        private List<GameObject> activeEnemies = new List<GameObject>();

        private void Awake()
        {
            if (enemyParent == null)
            {
                enemyParent = transform;
            }
        }

        /// <summary>
        /// Spawn a single enemy at a specific position.
        /// </summary>
        /// <param name="position">World position to spawn the enemy</param>
        /// <returns>The spawned enemy GameObject</returns>
        public GameObject SpawnEnemy(Vector3 position)
        {
            GameObject enemy = enemyPool.GetObject(position, Quaternion.identity);
            
            if (enemy != null)
            {
                enemy.transform.SetParent(enemyParent);
                activeEnemies.Add(enemy);
                Debug.Log($"Spawned enemy at {position}. Total active: {GetActiveEnemyCount()}");
            }

            return enemy;
        }

        /// <summary>
        /// Spawn a group of enemies at a spawn point.
        /// </summary>
        /// <param name="spawnPointIndex">Index of the spawn point to use</param>
        /// <param name="count">Number of enemies to spawn</param>
        public void SpawnEnemyGroup(int spawnPointIndex, int count)
        {
            if (spawnPoints == null || spawnPointIndex >= spawnPoints.Length)
            {
                Debug.LogWarning("Invalid spawn point index.");
                return;
            }

            Vector3 basePosition = spawnPoints[spawnPointIndex].position;
            SpawnEnemyGroupAtPosition(basePosition, count);
        }

        /// <summary>
        /// Spawn a group of enemies at a specific position.
        /// </summary>
        /// <param name="position">Center position for the group</param>
        /// <param name="count">Number of enemies to spawn</param>
        public void SpawnEnemyGroupAtPosition(Vector3 position, int count)
        {
            for (int i = 0; i < count; i++)
            {
                // Arrange enemies in a formation
                int gridSize = Mathf.CeilToInt(Mathf.Sqrt(count));
                int row = i / gridSize;
                int col = i % gridSize;

                Vector3 offset = new Vector3(
                    (col - gridSize / 2f) * groupSpacing,
                    0,
                    (row - gridSize / 2f) * groupSpacing
                );

                SpawnEnemy(position + offset);
            }
        }

        /// <summary>
        /// Return a specific enemy to the pool.
        /// </summary>
        /// <param name="enemy">The enemy to return</param>
        public void ReturnEnemy(GameObject enemy)
        {
            if (enemy != null)
            {
                enemyPool.ReturnObject(enemy);
                activeEnemies.Remove(enemy);
            }
        }

        /// <summary>
        /// Return all active enemies to the pool.
        /// </summary>
        public void ReturnAllToPool()
        {
            foreach (var enemy in activeEnemies)
            {
                if (enemy != null)
                {
                    enemyPool.ReturnObject(enemy);
                }
            }
            activeEnemies.Clear();
        }

        /// <summary>
        /// Get the count of currently active enemies.
        /// </summary>
        public int GetActiveEnemyCount()
        {
            // Clean up null references
            activeEnemies.RemoveAll(e => e == null || !e.activeSelf);
            return activeEnemies.Count;
        }

        /// <summary>
        /// Get all active enemy GameObjects.
        /// </summary>
        public List<GameObject> GetActiveEnemies()
        {
            activeEnemies.RemoveAll(e => e == null || !e.activeSelf);
            return new List<GameObject>(activeEnemies);
        }

        /// <summary>
        /// Check if any enemies are still active.
        /// </summary>
        public bool HasActiveEnemies()
        {
            return GetActiveEnemyCount() > 0;
        }
    }
}
