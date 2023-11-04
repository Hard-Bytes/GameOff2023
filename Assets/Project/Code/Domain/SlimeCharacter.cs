using System;
using UnityEngine;

namespace Project.Code.Domain
{
    [SelectionBase]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CapsuleCollider2D))]
    public class SlimeCharacter : MonoBehaviour
    {
        [Header("--> References")]
        [SerializeField] private SlimeCharacterInputReceiver inputReceiver;

        [Header("--> Values")] 
        [SerializeField] private float speedWalking = 3.0f;
        [SerializeField] private float speedRunning = 6.0f;
        
        [Tooltip("Se usará esto en vez del jumpforce, pero por ahora bien")]
        [SerializeField] private float jumpHeight = 2.0f;
        [SerializeField] private float jumpForce = 300.0f;

        private Vector2 _movementDirection; // valor normalizado
        private bool _isRunning;
        private Transform _transform;
        private Rigidbody2D _rigidbody;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            // Subscribirse a los eventos de input
            inputReceiver.OnJumpAction += OnJumpAction; 
            inputReceiver.OnRunActionStart += OnRunActionStart; 
            inputReceiver.OnRunActionEnd += OnRunActionEnd; 
            inputReceiver.OnMovementAction += OnMovementActionStart; 
        }

        private void FixedUpdate()
        {
            float speed = _isRunning ? speedRunning : speedWalking;
            Vector3 deltaMovement = _movementDirection * (speed * Time.fixedDeltaTime);
            _transform.position += deltaMovement;
        }

        private void OnDestroy()
        {
            // Desubscribirse a los eventos de input
            inputReceiver.OnJumpAction -= OnJumpAction; 
            inputReceiver.OnRunActionStart -= OnRunActionStart; 
            inputReceiver.OnRunActionEnd -= OnRunActionEnd; 
            inputReceiver.OnMovementAction -= OnMovementActionStart; 
        }

        private void OnJumpAction()
        {
            _rigidbody.AddForce(Vector2.up * jumpForce);
        }
        
        private void OnRunActionStart() => _isRunning = true;

        private void OnRunActionEnd() =>_isRunning = false;
        
        private void OnMovementActionStart(Vector2 value) => _movementDirection = value;
    }
}
