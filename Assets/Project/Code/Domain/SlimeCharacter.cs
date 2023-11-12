using System;
using System.Collections.Generic;
using UnityEngine;
using Project.Code.Domain.Events;
using Project.Code.Patterns.Events;
using Project.Code.Patterns.Services;
using Project.Code.Utils;
using UniRx;

namespace Project.Code.Domain
{
    [SelectionBase]
    [RequireComponent(typeof(Rigidbody2D))]
    public class SlimeCharacter : MonoBehaviour
    {
        [Header("--> References")]
        [SerializeField] private SlimeCharacterInputReceiver inputReceiver;
        [SerializeField] private SlimeCharacterMovementBehaviour movementBehaviour;
        [SerializeField] private SlimeCharacterJumpBehaviour jumpBehaviour;

        [Header("Health")]
        [SerializeField] private HealthSystem healthParameters;

        //[Header("--> Values")]
        //[SerializeField] private float speedWalking = 3.0f;
        //[SerializeField] private float speedRunning = 6.0f;
        // [SerializeField] private Size size = Size.Small;

        [Header("Size Parameters")]
        [SerializeField] private SlimeSizeVariable smallParameters;
        [SerializeField] private SlimeSizeVariable mediumParameters;
        [SerializeField] private SlimeSizeVariable bigParameters;
        private SlimeSizeVariable actualParameters;
        private Vector3 basicScale;

        private Vector2 _movementDirection; // valor normalizado
        private bool _isRunning;
        //private Transform _transform;
        //private Rigidbody2D _rigidbody;
        //private CapsuleCollider2D _capsule;

        private Vector2 _respawnPosition;
        private SlimeSize _respawnSize;

        private readonly List<IDisposable> _disposables = new List<IDisposable>();
/*
        private void Awake()
        {
            _transform = GetComponent<Transform>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _capsule = GetComponent<CapsuleCollider2D>();
        }
*/
        private void Start()
        {
            basicScale = gameObject.transform.localScale;

            // Subscribirse a los eventos
            // Input
            inputReceiver.OnJumpActionStart += OnJumpActionStart;
            inputReceiver.OnJumpActionEnd += OnJumpActionEnd;
            // inputReceiver.OnJumpAction += OnJumpAction;
            inputReceiver.OnRunActionStart += OnRunActionStart;
            inputReceiver.OnRunActionEnd += OnRunActionEnd;

            inputReceiver.OnMovementAction += OnMovementActionStart;

            // Size
            healthParameters.Size.Subscribe(OnSizeChanged).AddTo(_disposables);

            // Events
            var dispatcher = ServiceLocator.Instance.GetService<EventDispatcher>();
            dispatcher.Subscribe<CheckpointActivatedEvent>(OnCheckpointActivated);
            dispatcher.Subscribe<CharacterDiedEvent>(OnCharacterDeath);

            healthParameters.Initialize();

            var startCheckpointEvent = new CheckpointActivatedEvent {
                RespawnPosition = transform.position,
                RespawnSize = healthParameters.GetSize(),
                IsStartOfLevel = true
            };
            dispatcher.Trigger<CheckpointActivatedEvent>(startCheckpointEvent);
        }

        private void FixedUpdate()
        {
            movementBehaviour.UpdateMovement(_movementDirection);
        }

        private void OnDestroy()
        {
            // Desubscribirse a los eventos de input
            inputReceiver.OnJumpActionStart -= OnJumpActionStart;
            inputReceiver.OnJumpActionEnd -= OnJumpActionEnd;

            inputReceiver.OnRunActionStart -= OnRunActionStart;
            inputReceiver.OnRunActionEnd -= OnRunActionEnd;

            inputReceiver.OnMovementAction -= OnMovementActionStart;

            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }

            var dispatcher = ServiceLocator.Instance.GetService<EventDispatcher>();
            dispatcher.Unsubscribe<CheckpointActivatedEvent>(OnCheckpointActivated);
            dispatcher.Unsubscribe<CharacterDiedEvent>(OnCharacterDeath);
        }

        private void OnJumpActionStart() => jumpBehaviour.DoJump();
        private void OnJumpActionEnd() => jumpBehaviour.CancelJump();
        private void OnRunActionStart() => movementBehaviour.SetRunning(true);
        private void OnRunActionEnd() => movementBehaviour.SetRunning(false);

        private void OnMovementActionStart(Vector2 value) => _movementDirection = value;

        private void OnCheckpointActivated(GameEvent evt)
        {
            var eventData = (CheckpointActivatedEvent)evt;
            _respawnPosition = eventData.RespawnPosition;
            _respawnSize = eventData.RespawnSize;
        }

        private void OnCharacterDeath(GameEvent evt)
        {
            var eventData = (CharacterDiedEvent)evt;

            transform.position = this._respawnPosition;
            healthParameters.SetHPFromSize(_respawnSize);
            // TODO Reset player's speed/movement?
        }

        private void OnSizeChanged(SlimeSize newSize)
        {
            switch(newSize)
            {
                case SlimeSize.Small:
                    actualParameters = smallParameters;
                    gameObject.transform.localScale = basicScale;
                    break;
                case SlimeSize.Medium:
                    actualParameters = mediumParameters;
                    gameObject.transform.localScale = basicScale*2;
                    break;
                case SlimeSize.Big:
                    actualParameters = bigParameters;
                    gameObject.transform.localScale = basicScale*3;
                    break;
            }
        }

        public SlimeSize GetSize()
        {
            return healthParameters.GetSize();
        }

        public void Bounce()
        {
            // OnJumpActionStart();
        }

        public void ChangeHP(int valueChange, DamageSource source = DamageSource.None)
        {
            healthParameters.ChangeHP(valueChange);

            if(healthParameters.GetHealthPoints() <=0)
            {
                var dispatcher = ServiceLocator.Instance.GetService<EventDispatcher>();
                dispatcher.Trigger<CharacterDiedEvent>(new CharacterDiedEvent {Source = source});
            }
        }

        public void Kill(DamageSource source)
        {
            ChangeHP(-healthParameters.GetMaxHP(), source);
        }
    }
}
