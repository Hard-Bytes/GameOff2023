using System;
using UnityEngine;

namespace Project.Code.Domain
{
    public class RaycastGroundChecker : GroundCheckBehaviour
    {
        [Header("References")]

        [SerializeField] private new Transform transform;
        
        [Header("Collider Settings")]
        
        [Tooltip("Length of the ground-checking collider")] 
        [SerializeField] private float groundLength = 0.95f;
        
        [Tooltip("Distance between the ground-checking colliders")] 
        [SerializeField] private Vector3 colliderOffset;

        [Header("Layer Masks")]
        
        [Tooltip("Which layers are read as the ground")] 
        [SerializeField] private LayerMask groundLayer;
        
        private bool _isGrounded;

#if UNITY_EDITOR
        private void Reset()
        {
            transform = GetComponent<Transform>();
        }
#endif

        private void Update()
        {
            var position = transform.position;

            _isGrounded = Physics2D.Raycast(position + colliderOffset, Vector2.down, groundLength, groundLayer) 
                          || Physics2D.Raycast(position - colliderOffset, Vector2.down, groundLength, groundLayer);
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // Draw the ground colliders on screen for debug purposes
            var position = transform.position;
            Gizmos.color = _isGrounded ? Color.green : Color.red;
            
            Gizmos.DrawLine(position + colliderOffset, position + colliderOffset + Vector3.down * groundLength);
            Gizmos.DrawLine(position - colliderOffset, position - colliderOffset + Vector3.down * groundLength);
        }
#endif
        
        public override bool IsGrounded() => _isGrounded;
    }
}