using System;
using UnityEngine;

namespace Project.Code.Domain
{
    [SelectionBase]
    [RequireComponent(typeof(Rigidbody2D))]
    public class SlimeCharacter : MonoBehaviour
    {
        [Header("--> References")]
        [SerializeField] private SlimeCharacterInputReceiver inputReceiver;

        [Header("--> Values")] 
        [SerializeField] private float speedWalking = 3.0f;
        [SerializeField] private float speedRunning = 6.0f;
        [SerializeField] private float jumpHeight = 2.0f;

        private Vector2 _movementDirection; // valor normalizado
        private bool _isRunning;
        private Transform _transform;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
        }

        private void Start()
        {
            
            
            // Subscribirse a los eventos de input
            inputReceiver.OnJumpAction += OnJumpAction; 
            inputReceiver.OnRunActionStart += OnRunActionStart; 
            inputReceiver.OnRunActionEnd += OnRunActionEnd; 
            inputReceiver.OnMovementAction += OnMovementActionStart; 
        }

        private void Update()
        {
            float speed = _isRunning ? speedRunning : speedWalking;
            Vector3 deltaMovement = _movementDirection * (speed * Time.deltaTime);
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
            Debug.Log("I jumped lol");
        }
        
        private void OnRunActionStart() => _isRunning = true;

        private void OnRunActionEnd() =>_isRunning = false;
        
        private void OnMovementActionStart(Vector2 value) => _movementDirection = value;
    }
}
