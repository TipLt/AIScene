using UnityEngine;

namespace AIScene.Management
{
    /// <summary>
    /// Central game management for the Brainrot Runner game.
    /// Coordinates between player management, enemy management, and game flow.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private EnemyManager enemyManager;

        [Header("Game Settings")]
        [SerializeField] private int initialPlayerCount = 1;

        private bool isGameActive;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            StartGame();
        }

        /// <summary>
        /// Initialize and start the game.
        /// </summary>
        public void StartGame()
        {
            isGameActive = true;

            // Spawn initial player(s)
            if (playerManager != null && initialPlayerCount > 0)
            {
                playerManager.SpawnPlayers(initialPlayerCount);
            }

            Debug.Log("Game Started!");
        }

        /// <summary>
        /// Handle picking up a number modifier from the track.
        /// Called when player passes through a +N or -N gate.
        /// </summary>
        /// <param name="value">The modifier value (positive or negative)</param>
        public void OnNumberPickup(int value)
        {
            if (!isGameActive || playerManager == null) return;

            playerManager.ModifyPlayerCount(value);

            // Check for game over
            if (playerManager.GetActivePlayerCount() <= 0)
            {
                GameOver();
            }
        }

        /// <summary>
        /// Handle enemy encounter from the track.
        /// </summary>
        /// <param name="position">Position to spawn enemies</param>
        /// <param name="enemyCount">Number of enemies to spawn</param>
        public void OnEnemyEncounter(Vector3 position, int enemyCount)
        {
            if (!isGameActive || enemyManager == null) return;

            enemyManager.SpawnEnemyGroupAtPosition(position, enemyCount);
        }

        /// <summary>
        /// Called when a player unit is eliminated.
        /// </summary>
        public void OnPlayerEliminated()
        {
            if (!isGameActive || playerManager == null) return;

            // Check for game over
            if (playerManager.GetActivePlayerCount() <= 0)
            {
                GameOver();
            }
        }

        /// <summary>
        /// Called when an enemy is eliminated.
        /// </summary>
        public void OnEnemyEliminated()
        {
            // Additional logic for enemy elimination can be added here
            Debug.Log($"Enemy eliminated. Remaining: {enemyManager?.GetActiveEnemyCount() ?? 0}");
        }

        /// <summary>
        /// End the game.
        /// </summary>
        public void GameOver()
        {
            isGameActive = false;
            Debug.Log("Game Over!");

            // Cleanup
            playerManager?.ReturnAllToPool();
            enemyManager?.ReturnAllToPool();
        }

        /// <summary>
        /// Restart the game.
        /// </summary>
        public void RestartGame()
        {
            GameOver();
            StartGame();
        }

        /// <summary>
        /// Check if the game is currently active.
        /// </summary>
        public bool IsGameActive => isGameActive;
    }
}
