using UnityEngine;

namespace Project.Code.Domain
{
    public class ComplexSlimeCharacterMovement : SlimeCharacterMovementBehaviour
    {
        [Header("References")]
        [SerializeField] private new Transform transform;
        [SerializeField] private new Rigidbody2D rigidbody;
        [SerializeField] private GroundCheckBehaviour groundChecker;
        
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
        private bool _requestingMovement; // _pressingKey

        private void Awake()
        {
            // TODO: quitar esto cuando se implemente el ground check
            _onGround = true;
        }

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
                _requestingMovement = true;
            }
            else
            {
                _requestingMovement = false;
            }

            _desiredVelocity = new Vector2(_directionX, 0f) * Mathf.Max(maxSpeed - friction, 0f);
        }

        public override void UpdateMovement(Vector2 direction)
        {
            _directionX = direction.x;

            _velocity = rigidbody.velocity;
            _onGround = groundChecker.IsGrounded();
            
            // TODO: seguir

            if (useAcceleration)
            {
                RunWithAcceleration();
            }
            else
            {
                if (_onGround)
                {
                    RunWithoutAcceleration();
                }
                else
                {
                    RunWithAcceleration();
                }
            }
        }

        public override void Jump()
        {
            throw new System.NotImplementedException();
        }

        private void RunWithAcceleration()
        {
            //Set our acceleration, deceleration, and turn speed stats, based on whether we're on the ground on in the air

            _acceleration = _onGround ? maxAcceleration : maxAirAcceleration;
            _deceleration = _onGround ? maxDecceleration : maxAirDeceleration;
            _turnSpeed = _onGround ? maxTurnSpeed : maxAirTurnSpeed;

            if (_requestingMovement)
            {
                // If the sign (i.e. positive or negative) of our input direction doesn't match our movement, it means we're turning around and so should use the turn speed stat.
                if (Mathf.Sign(_directionX) != Mathf.Sign(_velocity.x))
                {
                    _maxSpeedChange = _turnSpeed * Time.deltaTime;
                }
                else
                {
                    // If they match, it means we're simply running along and so should use the acceleration stat
                    _maxSpeedChange = _acceleration * Time.deltaTime;
                }
            }
            else
            {
                // And if we're not pressing a direction at all, use the deceleration stat
                _maxSpeedChange = _deceleration * Time.deltaTime;
            }

            // Move our velocity towards the desired velocity, at the rate of the number calculated above
            _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, _maxSpeedChange);

            // Update the Rigidbody with this new velocity
            rigidbody.velocity = _velocity;
        }

        private void RunWithoutAcceleration()
        {
            // If we're not using acceleration and deceleration, just send our desired velocity (direction * max speed) to the Rigidbody
            _velocity.x = _desiredVelocity.x;

            rigidbody.velocity = _velocity;
        }
    }
}