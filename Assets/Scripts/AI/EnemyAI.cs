using UnityEngine;
using UnityEngine.AI;
using AIScene.Events;

namespace AIScene.AI
{
    /// <summary>
    /// Enemy AI component that uses NavMesh for pathfinding.
    /// Detects and chases the nearest player unit.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyAI : MonoBehaviour
    {
        [Header("Detection Settings")]
        [SerializeField] private float detectionRadius = 15f;
        [SerializeField] private float chaseUpdateInterval = 0.2f;
        [SerializeField] private LayerMask playerLayerMask;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float acceleration = 8f;
        [SerializeField] private float stoppingDistance = 0.5f;

        private NavMeshAgent navAgent;
        private Transform currentTarget;
        private float lastUpdateTime;
        private bool isChasing;

        // Reference to the player parent object for detection
        private static Transform playerParent;
        private static bool playerParentInitialized;
        private static bool eventSubscribed;

        private void Awake()
        {
            navAgent = GetComponent<NavMeshAgent>();
            SetupNavAgent();
            SubscribeToEvents();
            TryCachePlayerParent();
        }

        /// <summary>
        /// Subscribe to game events for decoupled communication.
        /// </summary>
        private static void SubscribeToEvents()
        {
            if (!eventSubscribed)
            {
                GameEvents.OnPlayerParentSet += HandlePlayerParentSet;
                eventSubscribed = true;
            }
        }

        /// <summary>
        /// Handle the player parent set event.
        /// </summary>
        private static void HandlePlayerParentSet(Transform parent)
        {
            SetPlayerParent(parent);
        }

        /// <summary>
        /// Attempt to cache the player parent reference once during initialization.
        /// </summary>
        private void TryCachePlayerParent()
        {
            if (!playerParentInitialized)
            {
                GameObject playerObj = GameObject.Find("Player");
                if (playerObj != null)
                {
                    playerParent = playerObj.transform;
                    playerParentInitialized = true;
                }
            }
        }

        private void OnEnable()
        {
            ResetAI();
        }

        private void OnDisable()
        {
            StopChasing();
        }

        /// <summary>
        /// Configure the NavMeshAgent with movement settings.
        /// </summary>
        private void SetupNavAgent()
        {
            navAgent.speed = moveSpeed;
            navAgent.acceleration = acceleration;
            navAgent.stoppingDistance = stoppingDistance;
            navAgent.updateRotation = true;
            navAgent.autoBraking = true;
        }

        /// <summary>
        /// Reset AI state when object is reactivated from pool.
        /// </summary>
        private void ResetAI()
        {
            isChasing = false;
            currentTarget = null;
            lastUpdateTime = 0f;

            if (navAgent != null && navAgent.isOnNavMesh)
            {
                navAgent.ResetPath();
                navAgent.isStopped = false;
            }
        }

        private void Update()
        {
            if (Time.time - lastUpdateTime >= chaseUpdateInterval)
            {
                lastUpdateTime = Time.time;
                UpdateTargetAndChase();
            }

            // Update destination if target exists and is valid
            if (isChasing && currentTarget != null && currentTarget.gameObject.activeSelf)
            {
                if (navAgent.isOnNavMesh && !navAgent.pathPending)
                {
                    navAgent.SetDestination(currentTarget.position);
                }
            }
            else if (isChasing)
            {
                // Target was lost or destroyed, find new target
                FindNewTarget();
            }
        }

        /// <summary>
        /// Update target detection and chase behavior.
        /// </summary>
        private void UpdateTargetAndChase()
        {
            if (currentTarget == null || !currentTarget.gameObject.activeSelf)
            {
                FindNewTarget();
            }
        }

        /// <summary>
        /// Find the nearest active player unit to chase.
        /// </summary>
        private void FindNewTarget()
        {
            // Try to cache player parent if not already done
            if (!playerParentInitialized)
            {
                TryCachePlayerParent();
            }

            if (playerParent == null)
            {
                StopChasing();
                return;
            }

            Transform nearestPlayer = null;
            float nearestDistance = detectionRadius;

            // Search through all child player units
            foreach (Transform child in playerParent)
            {
                if (!child.gameObject.activeSelf) continue;

                float distance = Vector3.Distance(transform.position, child.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPlayer = child;
                }
            }

            if (nearestPlayer != null)
            {
                SetTarget(nearestPlayer);
            }
            else
            {
                StopChasing();
            }
        }

        /// <summary>
        /// Set a new target to chase.
        /// </summary>
        /// <param name="target">The target transform to chase</param>
        public void SetTarget(Transform target)
        {
            currentTarget = target;
            isChasing = true;

            if (navAgent != null && navAgent.isOnNavMesh)
            {
                navAgent.isStopped = false;
                navAgent.SetDestination(target.position);
            }
        }

        /// <summary>
        /// Stop chasing and clear the current target.
        /// </summary>
        public void StopChasing()
        {
            isChasing = false;
            currentTarget = null;

            if (navAgent != null && navAgent.isOnNavMesh)
            {
                navAgent.ResetPath();
                navAgent.isStopped = true;
            }
        }

        /// <summary>
        /// Check if enemy is currently chasing a target.
        /// </summary>
        public bool IsChasing => isChasing;

        /// <summary>
        /// Get the current chase target.
        /// </summary>
        public Transform CurrentTarget => currentTarget;

        /// <summary>
        /// Set the player parent reference for all enemies.
        /// </summary>
        /// <param name="parent">The parent transform containing all player units</param>
        public static void SetPlayerParent(Transform parent)
        {
            playerParent = parent;
            playerParentInitialized = parent != null;
        }

        /// <summary>
        /// Update detection radius at runtime.
        /// </summary>
        /// <param name="radius">New detection radius</param>
        public void SetDetectionRadius(float radius)
        {
            detectionRadius = radius;
        }

        /// <summary>
        /// Update movement speed at runtime.
        /// </summary>
        /// <param name="speed">New movement speed</param>
        public void SetMoveSpeed(float speed)
        {
            moveSpeed = speed;
            if (navAgent != null)
            {
                navAgent.speed = speed;
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Draw detection radius in editor
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);

            // Draw line to current target
            if (currentTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, currentTarget.position);
            }
        }
    }
}
