using System;
using UnityEngine;

namespace Project.Code.Domain
{
    public class ComplexSlimeCharacterMovement : SlimeCharacterMovementBehaviour
    {
        [Header("References")]
        [SerializeField] private new Transform transform;
        [SerializeField] private new Rigidbody2D rigidbody;
        
        [Header("Movement Stats")]
        
        [Tooltip("Maximum movement speed")] 
        [SerializeField, Range(0f, 20f)] private float maxSpeed = 10f;
        
        [Tooltip("How fast to reach max speed")] 
        [SerializeField, Range(0f, 100f)] private float maxAcceleration = 52f;
        
        [Tooltip("How fast to stop after letting go")] 
        [SerializeField, Range(0f, 100f)] private float maxDecceleration = 52f;
        
        [Tooltip("How fast to stop when changing direction")] 
        [SerializeField, Range(0f, 100f)] public float maxTurnSpeed = 80f;
        
        [Tooltip("How fast to reach max speed when in mid-air")] 
        [SerializeField, Range(0f, 100f)] public float maxAirAcceleration;
        
        [Tooltip("How fast to stop in mid-air when no direction is used")]
        [SerializeField, Range(0f, 100f)] public float maxAirDeceleration;
        
        [Tooltip("How fast to stop when changing direction when in mid-air")] 
        [SerializeField, Range(0f, 100f)] public float maxAirTurnSpeed = 80f;
        
        [Tooltip("Friction to apply against movement on stick")] 
        [SerializeField] private float friction;
        
        [Tooltip("When false, the character will skip acceleration and deceleration and instantly move and stop")] 
        [SerializeField] private bool useAcceleration;
        
        [Header("Calculations")]
        private float _directionX;
        private Vector2 _desiredVelocity;
        private Vector2 _velocity;
        private float _maxSpeedChange;
        private float _acceleration;
        private float _deceleration;
        private float _turnSpeed;

        [Header("Current State")]
        private bool _onGround;
        private bool _pressingKey;

        private void Reset()
        {
            transform = GetComponent<Transform>(); 
            rigidbody = GetComponent<Rigidbody2D>(); 
        }

        private void Update()
        {
            if (_directionX != 0)
            {
                // Flipear el sprite vaya
                transform.localScale = new Vector3(_directionX > 0 ? 1 : -1, 1, 1);
            }
            
            _desiredVelocity = new Vector2(_directionX, 0f) * Mathf.Max(maxSpeed - friction, 0f);
        }

        public override void UpdateMovement(Vector2 direction)
        {
            _directionX = direction.x;

            _velocity = rigidbody.velocity;
            
            // TODO: seguir
        }

        public override void Jump()
        {
            throw new System.NotImplementedException();
        }
    }
}