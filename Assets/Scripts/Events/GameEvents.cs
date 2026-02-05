using UnityEngine;

namespace AIScene.Events
{
    /// <summary>
    /// Static event system for decoupled communication between game components.
    /// Allows PlayerManager to notify EnemyAI without direct references.
    /// </summary>
    public static class GameEvents
    {
        /// <summary>
        /// Event fired when the player parent transform is set.
        /// </summary>
        public static event System.Action<Transform> OnPlayerParentSet;

        /// <summary>
        /// Event fired when a player unit is eliminated.
        /// </summary>
        public static event System.Action<GameObject> OnPlayerEliminated;

        /// <summary>
        /// Event fired when an enemy is eliminated.
        /// </summary>
        public static event System.Action<GameObject> OnEnemyEliminated;

        /// <summary>
        /// Notify listeners that the player parent has been set.
        /// </summary>
        /// <param name="playerParent">The player parent transform</param>
        public static void NotifyPlayerParentSet(Transform playerParent)
        {
            OnPlayerParentSet?.Invoke(playerParent);
        }

        /// <summary>
        /// Notify listeners that a player was eliminated.
        /// </summary>
        /// <param name="player">The eliminated player</param>
        public static void NotifyPlayerEliminated(GameObject player)
        {
            OnPlayerEliminated?.Invoke(player);
        }

        /// <summary>
        /// Notify listeners that an enemy was eliminated.
        /// </summary>
        /// <param name="enemy">The eliminated enemy</param>
        public static void NotifyEnemyEliminated(GameObject enemy)
        {
            OnEnemyEliminated?.Invoke(enemy);
        }
    }
}
