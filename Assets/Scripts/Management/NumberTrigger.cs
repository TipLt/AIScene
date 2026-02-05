using UnityEngine;

namespace AIScene.Management
{
    /// <summary>
    /// Trigger component for number gates on the track (+2, -1, etc.)
    /// When player passes through, modifies the player count accordingly.
    /// </summary>
    public class NumberTrigger : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int value = 1;
        [SerializeField] private bool triggerOnce = true;

        private bool hasTriggered;

        private void OnTriggerEnter(Collider other)
        {
            if (triggerOnce && hasTriggered) return;

            // Check if it's the main player or a player unit
            if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
            {
                hasTriggered = true;
                
                // Notify game manager
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.OnNumberPickup(value);
                }

                Debug.Log($"Number trigger activated: {(value >= 0 ? "+" : "")}{value}");
            }
        }

        /// <summary>
        /// Set the value of this number trigger.
        /// </summary>
        /// <param name="newValue">The new value (positive or negative)</param>
        public void SetValue(int newValue)
        {
            value = newValue;
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
