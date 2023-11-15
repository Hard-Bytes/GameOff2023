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
        [SerializeField] private GameObject slimeScrapPrefab;

        [Header("Health")]
        [SerializeField] private HealthComponent healthParameters;

        [Header("Divide ability")]
        [SerializeField, Range(1, 100)] private int divideCost;
        [SerializeField] private bool divideScrapLaunchForward = false;
        [SerializeField] private float divideScrapLaunchOffset = 2.0f;
        //Might disappear when we implement a proper state machine
        [SerializeField, Range(0.1f, 4f)] private float divideCooldownSec = 0.5f;
        private float _divideCooldownCounter = 0.0f;

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

        [Header("Knockback Parameters")]
        [SerializeField, Range(0f, 90f)] private float angleAttack;
        [SerializeField, Range(0f, 100f)] private float knockback = 1.0f;
        [SerializeField, Range(0f, 10f)] private float invincibleTime;
        [SerializeField] private bool knockbackWhileInvincible;
        private float _invincible = 0;

        private Vector2 _movementDirection; // valor normalizado
        private bool _isRunning;
        private bool _lookingLeft = false;

        private Transform _transform;
        //private Rigidbody2D _rigidbody;
        //private CapsuleCollider2D _capsule;

        private Vector2 _respawnPosition;
        private SlimeSize _respawnSize;

        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            // _rigidbody = GetComponent<Rigidbody2D>();
            // _capsule = GetComponent<CapsuleCollider2D>();
        }

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

            inputReceiver.OnDivideAction += OnDivide;

            // Size
            healthParameters.Size.Subscribe(OnSizeChanged).AddTo(_disposables);

            // Events
            var dispatcher = ServiceLocator.Instance.GetService<EventDispatcher>();
            dispatcher.Subscribe<CheckpointActivatedEvent>(OnCheckpointActivated);

            healthParameters.Initialize(divideCost);

            var startCheckpointEvent = new CheckpointActivatedEvent {
                RespawnPosition = transform.position,
                RespawnSize = healthParameters.GetSize(),
                IsStartOfLevel = true
            };
            dispatcher.Trigger<CheckpointActivatedEvent>(startCheckpointEvent);
        }

        private void FixedUpdate()
        {
            _transform.localScale = new Vector3(
                _movementDirection.x > 0 ? Mathf.Abs(_transform.localScale.x) : -Mathf.Abs(_transform.localScale.x),
                _transform.localScale.y,
                _transform.localScale.z
            );
            movementBehaviour.UpdateMovement(_movementDirection);

            //TODO I'm starting to see a pattern here...
            if(_invincible > 0) _invincible -= Time.fixedDeltaTime;
            if(_divideCooldownCounter > 0) _divideCooldownCounter -= Time.fixedDeltaTime;

            if(_movementDirection.x != 0.0f)
            {
                _lookingLeft = _movementDirection.x < 0;
            }
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
        }

        private void OnJumpActionStart() => jumpBehaviour.DoJump();
        private void OnJumpActionEnd() => jumpBehaviour.CancelJump();
        private void OnRunActionStart() => movementBehaviour.SetRunning(true);
        private void OnRunActionEnd() => movementBehaviour.SetRunning(false);

        private void OnMovementActionStart(Vector2 value) => _movementDirection = value;

        private void OnDivide()
        {
            if(jumpBehaviour.OnGround() && _divideCooldownCounter <= 0.0f)
            {
                _divideCooldownCounter = divideCooldownSec;

                if(healthParameters.GetHealthPoints() > divideCost)
                {
                    healthParameters.ChangeHP(-divideCost);
                    var scrapObj = GameObject.Instantiate(slimeScrapPrefab);

                    if(scrapObj.TryGetComponent(out Collider2D scrapCollider))
                    {
                        scrapObj.transform.position = transform.position + new Vector3(0, scrapCollider.bounds.size.y / 2, 0);
                    }
                    else
                    {
                        scrapObj.transform.position = transform.position;
                    }

                    if(!scrapObj.TryGetComponent(out SlimeScrap scrapData)) return;

                    bool launchForward = divideScrapLaunchForward;
                    if(_lookingLeft) launchForward = !launchForward;
                    scrapData.SetScrapValue(divideCost);
                    scrapData.SetOffsetDestination(new Vector2(
                        launchForward? divideScrapLaunchOffset : -divideScrapLaunchOffset
                        , 0
                    ));
                }
                else
                {
                    // TODO Play an "error"/"unavailable" sound maybe? And make UI bar blink or something
                }
            }
        }

        private void OnCheckpointActivated(GameEvent evt)
        {
            var eventData = (CheckpointActivatedEvent)evt;
            _respawnPosition = eventData.RespawnPosition;
            _respawnSize = eventData.RespawnSize;
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

        public void ChangeHP(int valueChange, DamageSource source = DamageSource.None, bool ignoreInvincibility = false)
        {
            if (_invincible <= 0 || ignoreInvincibility)
            {
                if(!ignoreInvincibility) _invincible = invincibleTime;
                healthParameters.ChangeHP(valueChange);

                if (healthParameters.GetHealthPoints() <= 0)
                {
                    // Dead
                    _transform.position = _respawnPosition;
                    healthParameters.SetHPFromSize(_respawnSize);
                    // TODO Reset player's speed/movement?

                    var dispatcher = ServiceLocator.Instance.GetService<EventDispatcher>();
                    dispatcher.Trigger<CharacterDiedEvent>(new CharacterDiedEvent { Source = source });
                }
            }
        }

        public void Kill(DamageSource source)
        {
            ChangeHP(-healthParameters.GetMaxHP(), source, true);
        }
        public void Knockback(Vector2 damageInput)
        {
            if(_invincible <= 0 || knockbackWhileInvincible)
            {
                int diretion = transform.position.x > damageInput.x ? 1 : -1;
                Vector2 movement = new Vector2(Mathf.Cos(angleAttack * Mathf.Deg2Rad) * diretion, Mathf.Sin(angleAttack * Mathf.Deg2Rad)) * knockback;

                Debug.Log(movement);
                //movementBehaviour.UpdateMovement(movement);
                GetComponent<Rigidbody2D>().velocity = movement;
            }
        }
    }
}
