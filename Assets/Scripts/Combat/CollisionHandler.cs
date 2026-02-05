using UnityEngine;
using UnityEngine.Events;

namespace AIScene.Combat
{
    /// <summary>
    /// Handles collision-based elimination between player units and enemy units.
    /// When a player child collides with an enemy child, both are deactivated (hidden).
    /// </summary>
    public class CollisionHandler : MonoBehaviour
    {
        [Header("Collision Settings")]
        [SerializeField] private string enemyTag = "Enemy";
        [SerializeField] private string playerTag = "Player";
        [SerializeField] private bool isEnemy;

        [Header("Events")]
        public UnityEvent<GameObject> OnEliminated;
        public UnityEvent<GameObject, GameObject> OnCollisionElimination;

        private bool isEliminated;

        private void OnEnable()
        {
            isEliminated = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            HandleCollision(other.gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            HandleCollision(collision.gameObject);
        }

        /// <summary>
        /// Process collision and handle elimination logic.
        /// </summary>
        /// <param name="other">The other game object involved in collision</param>
        private void HandleCollision(GameObject other)
        {
            if (isEliminated) return;

            // Check if this is a valid elimination collision
            if (isEnemy && other.CompareTag(playerTag))
            {
                ProcessElimination(other);
            }
            else if (!isEnemy && other.CompareTag(enemyTag))
            {
                ProcessElimination(other);
            }
        }

        /// <summary>
        /// Process mutual elimination between this unit and the other.
        /// </summary>
        /// <param name="other">The other unit to eliminate</param>
        private void ProcessElimination(GameObject other)
        {
            // Check if the other unit has a collision handler
            CollisionHandler otherHandler = other.GetComponent<CollisionHandler>();
            
            // Mark both as eliminated to prevent duplicate processing
            isEliminated = true;
            if (otherHandler != null)
            {
                otherHandler.isEliminated = true;
            }

            // Fire events before deactivation
            OnEliminated?.Invoke(other);
            OnCollisionElimination?.Invoke(gameObject, other);

            // Notify the other unit's handler
            if (otherHandler != null)
            {
                otherHandler.OnEliminated?.Invoke(gameObject);
            }

            // Deactivate both units (hide them)
            gameObject.SetActive(false);
            other.SetActive(false);

            Debug.Log($"Elimination: {gameObject.name} and {other.name} eliminated each other.");
        }

        /// <summary>
        /// Manually trigger elimination (useful for external systems).
        /// </summary>
        public void Eliminate()
        {
            if (isEliminated) return;
            
            isEliminated = true;
            OnEliminated?.Invoke(null);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Check if this unit has been eliminated.
        /// </summary>
        public bool IsEliminated => isEliminated;

        /// <summary>
        /// Reset elimination state (called when returning to pool).
        /// </summary>
        public void ResetElimination()
        {
            isEliminated = false;
        }
    }
}
