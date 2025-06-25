using System.Collections.Generic;
using UnityEngine;

namespace Interaction
{
    public class InteractionDetector : MonoBehaviour
    {
        [Header("Detection Settings")]
        [SerializeField] private float interactionRange = 3f;
        [SerializeField] private LayerMask interactionLayers = -1;
        [SerializeField] private int maxInteractables = 10;
        
        [Header("Raycast Settings")]
        [SerializeField] private Transform raycastOrigin;
        [SerializeField] private bool useSphereCast = true;
        [SerializeField] private float sphereCastRadius = 0.5f;
        
        [Header("Line of Sight Settings")]
        [SerializeField] private float fieldOfViewAngle = 30f;
        [SerializeField] private bool requireDirectLineOfSight = true;
        
        [Header("Debug")]
        [SerializeField] private bool showDebug = false;
        
        public System.Action<IInteractable> OnInteractableChanged;
        
        private IInteractable currentInteractable;
        private readonly Collider[] overlapResults = new Collider[10];
        private readonly List<IInteractable> nearbyInteractables = new List<IInteractable>();
        
        public IInteractable CurrentInteractable => currentInteractable;
        public float InteractionRange => interactionRange;
        
        private void Start()
        {
            if (raycastOrigin == null)
                raycastOrigin = Camera.main?.transform ?? transform;
        }
        
        private void Update()
        {
            DetectInteractables();
        }
        
        private void DetectInteractables()
        {
            IInteractable newInteractable = null;
            
            // Try to find interactable with direct raycast first
            RaycastHit hit;
            bool didHit = false;
            
            if (useSphereCast)
            {
                didHit = Physics.SphereCast(raycastOrigin.position, sphereCastRadius, raycastOrigin.forward, 
                    out hit, interactionRange, interactionLayers);
            }
            else
            {
                didHit = Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, 
                    out hit, interactionRange, interactionLayers);
            }
            
            if (didHit)
            {
                var hitInteractable = hit.collider.GetComponent<IInteractable>();
                if (hitInteractable != null && hitInteractable.CanInteract)
                {
                    newInteractable = hitInteractable;
                }
            }
            
            // Only use proximity detection if no line-of-sight object was found
            if (newInteractable == null)
            {
                newInteractable = GetClosestInteractableInLineOfSight();
            }
            
            // Update current interactable
            if (newInteractable != currentInteractable)
            {
                currentInteractable?.OnInteractionExit(gameObject);
                currentInteractable = newInteractable;
                currentInteractable?.OnInteractionEnter(gameObject);
                OnInteractableChanged?.Invoke(currentInteractable);
            }
        }
        
        private IInteractable GetClosestInteractableInLineOfSight()
        {
            int hitCount = Physics.OverlapSphereNonAlloc(raycastOrigin.position, interactionRange, 
                overlapResults, interactionLayers);
            
            if (hitCount == 0) return null;
            
            nearbyInteractables.Clear();
            
            // Get valid interactables in line of sight
            for (int i = 0; i < hitCount; i++)
            {
                var interactable = overlapResults[i].GetComponent<IInteractable>();
                if (interactable != null && interactable.CanInteract && IsInLineOfSight(interactable))
                {
                    nearbyInteractables.Add(interactable);
                }
            }
            
            if (nearbyInteractables.Count == 0) return null;
            
            // Find closest interactable
            IInteractable closest = null;
            float closestDistance = float.MaxValue;
            
            foreach (var interactable in nearbyInteractables)
            {
                float distance = Vector3.Distance(raycastOrigin.position, interactable.Transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = interactable;
                }
            }
            
            return closest;
        }
        
        private bool IsInLineOfSight(IInteractable interactable)
        {
            Vector3 directionToTarget = (interactable.Transform.position - raycastOrigin.position).normalized;
            
            // Check if in field of view
            float dotProduct = Vector3.Dot(raycastOrigin.forward, directionToTarget);
            float angleThreshold = Mathf.Cos(fieldOfViewAngle * 0.5f * Mathf.Deg2Rad);
            if (dotProduct < angleThreshold) return false;
            
            if (!requireDirectLineOfSight) return true;
            
            // Check if there's a clear line of sight
            float distance = Vector3.Distance(raycastOrigin.position, interactable.Transform.position);
            if (Physics.Raycast(raycastOrigin.position, directionToTarget, out RaycastHit hit, distance, interactionLayers))
            {
                return hit.collider.GetComponent<IInteractable>() == interactable;
            }
            
            return false;
        }
        
        public bool TryInteract()
        {
            Debug.Log($"Attempting interaction with: {currentInteractable?.InteractionText}");
            if (currentInteractable != null && currentInteractable.CanInteract)
            {
                currentInteractable.Interact(gameObject);
                return true;
            }
            return false;
        }
        
        private void OnDrawGizmosSelected()
        {
            if (!showDebug || raycastOrigin == null) return;
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(raycastOrigin.position, interactionRange);
            
            Gizmos.color = useSphereCast ? Color.green : Color.red;
            Gizmos.DrawRay(raycastOrigin.position, raycastOrigin.forward * interactionRange);
            
            if (useSphereCast)
            {
                Gizmos.DrawWireSphere(raycastOrigin.position + raycastOrigin.forward * interactionRange, sphereCastRadius);
            }
        }
    }
}
