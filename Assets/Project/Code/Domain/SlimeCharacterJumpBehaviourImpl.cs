using System;
using NaughtyAttributes;
using UnityEngine;

namespace Project.Code.Domain
{
    public class SlimeCharacterJumpBehaviourImpl : SlimeCharacterJumpBehaviour
    {
        [Header("Components")]
        
        [SerializeField] private new Rigidbody2D rigidbody2D;
        [SerializeField] private GroundCheckBehaviour groundChecker;

        [Header("Jumping Stats")]
        
        [Tooltip("Maximum jump height")] 
        [SerializeField, Range(2f, 5.5f)] private float jumpHeight = 7.3f;

        [Tooltip("How long it takes to reach that height before coming back down")] 
        [SerializeField, Range(0.2f, 1.25f)] private float timeToJumpApex;
        
        [Tooltip("Gravity multiplier to apply when going up")] 
        [SerializeField, Range(0f, 5f)] private float upwardMovementMultiplier = 1f;
        
        [Tooltip("Gravity multiplier to apply when coming down")] 
        [SerializeField, Range(1f, 10f)] private float downwardMovementMultiplier = 6.17f;
        
        [Tooltip("How many times can you jump in the air?")] 
        [SerializeField, Range(0, 1)] private int maxAirJumps = 0;

        [Header("Options")]
        
        [Tooltip("Should the character drop when you let go of jump?")] 
        [SerializeField] private bool variableJumpHeight;
        
        [Tooltip("Gravity multiplier when you let go of jump")] 
        [SerializeField, Range(1f, 10f)] private float jumpCutOff;
        
        [Tooltip("The fastest speed the character can fall")] 
        [SerializeField] private float speedLimit;
        
        [Tooltip("How long should coyote time last?")] 
        [SerializeField, Range(0f, 0.3f)] private float coyoteTime = 0.15f;
        
        [Tooltip("How far from ground should we cache your jump?")] 
        [SerializeField, Range(0f, 0.3f)] private float jumpBuffer = 0.15f;

        [Header("Constants")]
        
        [ShowNonSerializedField] private const float DefaultGravityScale = 1.0f; 
        
        [Header("Calculations")]
        
        [ShowNonSerializedField] private Vector2 _velocity;
        [ShowNonSerializedField] private float _jumpSpeed;
        [ShowNonSerializedField] private float _gravityMultiplier;

        [Header("Current State")]
        
        [ShowNonSerializedField] private bool _canJumpAgain;
        [ShowNonSerializedField] private bool _onGround;
        [ShowNonSerializedField] private bool _desiredJump;
        [ShowNonSerializedField] private float _jumpBufferCounter;
        [ShowNonSerializedField] private float _coyoteTimeCounter;
        [ShowNonSerializedField] private bool _pressingJump;
        [ShowNonSerializedField] private bool _currentlyJumping;
        
#if UNITY_EDITOR
        private void Reset()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
            groundChecker = GetComponent<GroundCheckBehaviour>();
        }
#endif

        private void Update()
        {
            SetPhysicsGravityScale();

            // Check if we're on ground, using Kit's Ground script
            _onGround = groundChecker.IsGrounded();

            // Jump buffer allows us to queue up a jump, which will play when we next hit the ground
            if (jumpBuffer > 0)
            {
                // Instead of immediately turning off "desireJump", start counting up...
                // All the while, the DoAJump function will repeatedly be fired off
                if (_desiredJump)
                {
                    _jumpBufferCounter += Time.deltaTime;

                    if (_jumpBufferCounter > jumpBuffer)
                    {
                        // If time exceeds the jump buffer, turn off "desireJump"
                        _desiredJump = false;
                        _jumpBufferCounter = 0;
                    }
                }
            }

            // If we're not on the ground and we're not currently jumping, that means we've stepped off the edge of a platform.
            // So, start the coyote time counter...
            if (!_currentlyJumping && !_onGround)
            {
                _coyoteTimeCounter += Time.deltaTime;
            }
            else
            {
                // Reset it when we touch the ground, or jump
                _coyoteTimeCounter = 0;
            }
        }

        private void FixedUpdate()
        {
            // Get velocity from Kit's Rigidbody 
            _velocity = rigidbody2D.velocity;

            // Keep trying to do a jump, for as long as desiredJump is true
            if (_desiredJump)
            {
                DoAJump();
                rigidbody2D.velocity = _velocity;

                // Skip gravity calculations this frame, so currentlyJumping doesn't turn off
                // This makes sure you can't do the coyote time double jump bug
                return;
            }

            CalculateGravity();
        }

        private void DoAJump()
        {
            // Create the jump, provided we are on the ground, in coyote time, or have a double jump available
            if (_onGround 
                || (_coyoteTimeCounter > 0.03f && _coyoteTimeCounter < coyoteTime) 
                || _canJumpAgain)
            {
                _desiredJump = false;
                _jumpBufferCounter = 0;
                _coyoteTimeCounter = 0;

                // If we have double jump on, allow us to jump again (but only once)
                _canJumpAgain = (maxAirJumps == 1 && _canJumpAgain == false);

                // Determine the power of the jump, based on our gravity and stats
                _jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * rigidbody2D.gravityScale * jumpHeight);

                // If Kit is moving up or down when she jumps (such as when doing a double jump), change the jumpSpeed;
                // This will ensure the jump is the exact same strength, no matter your velocity.
                if (_velocity.y > 0f)
                {
                    _jumpSpeed = Mathf.Max(_jumpSpeed - _velocity.y, 0f);
                }
                else if (_velocity.y < 0f)
                {
                    _jumpSpeed += Mathf.Abs(rigidbody2D.velocity.y);
                }

                // Apply the new jumpSpeed to the velocity. It will be sent to the Rigidbody in FixedUpdate;
                _velocity.y += _jumpSpeed;
                _currentlyJumping = true;

                
                // TODO: en vez de llamar a los efectos, se puede hacer algo como
                // JumpStartedCallback?.Invoke();
                // Y la gente se suscribe desde fuera
                
                // if (juice != null)
                // {
                //     //Apply the jumping effects on the juice script
                //     juice.jumpEffects();
                // }
            }

            if (jumpBuffer == 0)
            {
                // If we don't have a jump buffer, then turn off desiredJump immediately after hitting jumping
                _desiredJump = false;
            }
        }

        private void SetPhysicsGravityScale() // setPhysics
        {
            // Determine the character's gravity scale, using the stats provided. Multiply it by a gravMultiplier, used later
            Vector2 newGravity = new Vector2(0, (-2 * jumpHeight) / (timeToJumpApex * timeToJumpApex));
            var newGravityScale = (newGravity.y / Physics2D.gravity.y) * _gravityMultiplier;
            rigidbody2D.gravityScale = newGravityScale;
        }
        
        private void CalculateGravity()
        {
            // We change the character's gravity based on her Y direction

            // If Kit is going up...
            if (rigidbody2D.velocity.y > 0.01f)
            {
                if (_onGround)
                {
                    // Don't change it if Kit is stood on something (such as a moving platform)
                    _gravityMultiplier = DefaultGravityScale;
                }
                else
                {
                    // If we're using variable jump height...)
                    if (variableJumpHeight)
                    {
                        // Apply upward multiplier if player is rising and holding jump
                        if (_pressingJump && _currentlyJumping)
                        {
                            _gravityMultiplier = upwardMovementMultiplier;
                        }
                        // But apply a special downward multiplier if the player lets go of jump
                        else
                        {
                            _gravityMultiplier = jumpCutOff;
                        }
                    }
                    else
                    {
                        _gravityMultiplier = upwardMovementMultiplier;
                    }
                }
            }

            // Else if going down...
            else if (rigidbody2D.velocity.y < -0.01f)
            {

                if (_onGround)
                // Don't change it if Kit is stood on something (such as a moving platform)
                {
                    _gravityMultiplier = DefaultGravityScale;
                }
                else
                {
                    // Otherwise, apply the downward gravity multiplier as Kit comes back to Earth
                    _gravityMultiplier = downwardMovementMultiplier;
                }

            }
            // Else not moving vertically at all
            else
            {
                if (_onGround)
                {
                    _currentlyJumping = false;
                }

                _gravityMultiplier = DefaultGravityScale;
            }

            // Set the character's Rigidbody's velocity
            // But clamp the Y variable within the bounds of the speed limit, for the terminal velocity assist option
            rigidbody2D.velocity = new Vector3(_velocity.x, Mathf.Clamp(_velocity.y, -speedLimit, 100));
        }
        
        public override void DoJump()
        {
            // Esta condición inhabilita el jump buffer, pero es que hay un bug ahora importante de que rebotas
            // Y puedes saltar hacia abajo
            if (!_currentlyJumping)
            {
                _desiredJump = true;
                _pressingJump = true;
            }
        }

        public override void CancelJump()
        {
            _pressingJump = false;
        }
    }
}