using UnityEngine;
using AIScene.Pooling;
using AIScene.Events;
using System.Collections.Generic;

namespace AIScene.Management
{
    /// <summary>
    /// Manages player units including spawning based on +/- numbers from the track.
    /// Uses object pooling for efficient unit management.
    /// </summary>
    public class PlayerManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ObjectPool playerPool;
        [SerializeField] private Transform playerParent;

        [Header("Spawn Settings")]
        [SerializeField] private Vector3 spawnOffset = new Vector3(0, 0, 1f);
        [SerializeField] private float unitSpacing = 0.5f;

        private List<GameObject> activeUnits = new List<GameObject>();

        private void Awake()
        {
            // Initialize with one player unit if none exist
            if (playerParent == null)
            {
                playerParent = transform;
            }
        }

        private void Start()
        {
            // Notify listeners about the player parent via event system
            GameEvents.NotifyPlayerParentSet(playerParent);
        }

        /// <summary>
        /// Modify player count based on a number value.
        /// Positive values spawn more players, negative values remove players.
        /// </summary>
        /// <param name="value">The number to add or subtract (e.g., +2, -1)</param>
        public void ModifyPlayerCount(int value)
        {
            if (value > 0)
            {
                SpawnPlayers(value);
            }
            else if (value < 0)
            {
                RemovePlayers(Mathf.Abs(value));
            }
        }

        /// <summary>
        /// Spawn a specified number of player units.
        /// </summary>
        /// <param name="count">Number of units to spawn</param>
        public void SpawnPlayers(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 spawnPosition = CalculateSpawnPosition();
                GameObject unit = playerPool.GetObject(spawnPosition, Quaternion.identity);
                
                if (unit != null)
                {
                    unit.transform.SetParent(playerParent);
                    activeUnits.Add(unit);
                }
            }

            Debug.Log($"Spawned {count} player units. Total active: {GetActivePlayerCount()}");
        }

        /// <summary>
        /// Remove a specified number of player units.
        /// </summary>
        /// <param name="count">Number of units to remove</param>
        public void RemovePlayers(int count)
        {
            int removed = 0;
            for (int i = activeUnits.Count - 1; i >= 0 && removed < count; i--)
            {
                GameObject unit = activeUnits[i];
                if (unit != null && unit.activeSelf)
                {
                    playerPool.ReturnObject(unit);
                    activeUnits.RemoveAt(i);
                    removed++;
                }
            }

            // Clean up null references
            activeUnits.RemoveAll(u => u == null || !u.activeSelf);

            Debug.Log($"Removed {removed} player units. Total active: {GetActivePlayerCount()}");
        }

        /// <summary>
        /// Calculate spawn position for a new player unit.
        /// </summary>
        private Vector3 CalculateSpawnPosition()
        {
            Vector3 basePosition = playerParent.position + spawnOffset;
            int activeCount = GetActivePlayerCount();
            
            // Arrange units in a grid formation
            int gridSize = Mathf.CeilToInt(Mathf.Sqrt(activeCount + 1));
            int row = activeCount / gridSize;
            int col = activeCount % gridSize;

            return basePosition + new Vector3(col * unitSpacing, 0, row * unitSpacing);
        }

        /// <summary>
        /// Get the count of currently active player units.
        /// </summary>
        public int GetActivePlayerCount()
        {
            // Clean up null references and count active units
            activeUnits.RemoveAll(u => u == null || !u.activeSelf);
            return activeUnits.Count;
        }

        /// <summary>
        /// Return all player units to the pool.
        /// </summary>
        public void ReturnAllToPool()
        {
            foreach (var unit in activeUnits)
            {
                if (unit != null)
                {
                    playerPool.ReturnObject(unit);
                }
            }
            activeUnits.Clear();
        }

        /// <summary>
        /// Get a list of all active player unit transforms.
        /// </summary>
        public List<Transform> GetActivePlayerTransforms()
        {
            List<Transform> transforms = new List<Transform>();
            foreach (var unit in activeUnits)
            {
                if (unit != null && unit.activeSelf)
                {
                    transforms.Add(unit.transform);
                }
            }
            return transforms;
        }
    }
}
