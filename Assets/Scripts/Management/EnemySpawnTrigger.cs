using UnityEngine;

namespace AIScene.Management
{
    /// <summary>
    /// Trigger component for spawning enemies when the player reaches a certain point.
    /// Place this on the track to spawn enemies when players pass through.
    /// </summary>
    public class EnemySpawnTrigger : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int enemyCount = 3;
        [SerializeField] private Vector3 spawnOffset = new Vector3(0, 0, 5f);
        [SerializeField] private bool triggerOnce = true;

        private bool hasTriggered;

        private void OnTriggerEnter(Collider other)
        {
            if (triggerOnce && hasTriggered) return;

            // Check if it's the main player or a player unit
            if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
            {
                hasTriggered = true;
                SpawnEnemies();
            }
        }

        /// <summary>
        /// Spawn enemies at the configured location.
        /// </summary>
        private void SpawnEnemies()
        {
            Vector3 spawnPosition = transform.position + spawnOffset;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnEnemyEncounter(spawnPosition, enemyCount);
            }

            Debug.Log($"Enemy spawn triggered: {enemyCount} enemies at {spawnPosition}");
        }

        /// <summary>
        /// Set the number of enemies to spawn.
        /// </summary>
        /// <param name="count">Number of enemies</param>
        public void SetEnemyCount(int count)
        {
            enemyCount = count;
        }

        /// <summary>
        /// Reset the trigger to allow it to activate again.
        /// </summary>
        public void ResetTrigger()
        {
            hasTriggered = false;
        }
    }
}
