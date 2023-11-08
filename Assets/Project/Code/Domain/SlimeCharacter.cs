using System;
using UnityEngine;
using Project.Code.Utils;

namespace Project.Code.Domain
{
    [SelectionBase]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CapsuleCollider2D))]
    public class SlimeCharacter : MonoBehaviour
    {
        [Header("--> References")]
        [SerializeField] private SlimeCharacterInputReceiver inputReceiver;
        [SerializeField] private SlimeCharacterMovementBehaviour movementBehaviour;
        
        [Header("Health")]
        [SerializeField] private HealthSystem healthParameters;
        
        [Header("--> Values")] 
        [SerializeField] private Size size = Size.Small;

        [Header("Size Parameters")]
        [SerializeField] private SlimeSizeVariable smallParameters;
        [SerializeField] private SlimeSizeVariable mediumParameters;
        [SerializeField] private SlimeSizeVariable bigParameters;
        private SlimeSizeVariable actualParameters;

        private Vector2 _movementDirection; // valor normalizado
        private bool _isRunning;

        private void Start()
        {
            // Subscribirse a los eventos de input
            inputReceiver.OnJumpAction += OnJumpAction; 
            inputReceiver.OnRunActionStart += OnRunActionStart; 
            inputReceiver.OnRunActionEnd += OnRunActionEnd; 
            inputReceiver.OnMovementAction += OnMovementActionStart;
            changeParameters();
            healthParameters.initialize();
        }

        private void FixedUpdate()
        {
            movementBehaviour.UpdateMovement(_movementDirection);
        }

        private void OnDestroy()
        {
            // Desubscribirse a los eventos de input
            inputReceiver.OnJumpAction -= OnJumpAction; 
            inputReceiver.OnRunActionStart -= OnRunActionStart; 
            inputReceiver.OnRunActionEnd -= OnRunActionEnd; 
            inputReceiver.OnMovementAction -= OnMovementActionStart; 
        }

        private void OnJumpAction() => movementBehaviour.Jump();
        
        private void OnRunActionStart() => movementBehaviour.SetRunning(true);

        private void OnRunActionEnd() => movementBehaviour.SetRunning(false);
        
        private void OnMovementActionStart(Vector2 value) => _movementDirection = value;

        public Size GetSize()
        {
            return size;
        }

        public void Bounce()
        {
            OnJumpAction();
        }

        public void ChangeHP(int valueChange)
        {
            Size newSize = healthParameters.changeHP(valueChange);
            
            if(newSize != size)
            {
                size = newSize;
                changeParameters();
            }
            
            if(healthParameters.GetHeathPoints() <= 0)
            {
                Destroy(gameObject);
            }
        }

        public void changeParameters()
        {
            // Para dedado, ¿por qué no hacer un diccionario de tipo de size a healthParamerters?
            
            switch(size)
            {
                case Size.Small:
                    actualParameters = smallParameters;
                    break;
                case Size.Medium:
                    actualParameters = mediumParameters;
                    break;
                case Size.Big:
                    actualParameters = bigParameters;
                    break;
            }
        }
    }
}
