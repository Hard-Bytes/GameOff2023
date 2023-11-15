using System;
using NaughtyAttributes;
using Project.Code.Domain.Events;
using Project.Code.Patterns.Events;
using Project.Code.Patterns.Services;
using UnityEngine;

namespace Project.Code.Domain
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Collectable : MonoBehaviour
    {
        [SerializeField] private int points;
        [SerializeField,MinValue(1)] private float hitboxMultiplier = 1f;
        [SerializeField] private bool isBigCollectable;
        private bool _checkPointCollected;
        private CoinCollectedEvent _coinCollectedEvent;

        private void Awake()
        {
            _coinCollectedEvent = new CoinCollectedEvent
            {
                points = points,
                isBigCoin = isBigCollectable,
            };
            GetComponent<BoxCollider2D>().size *= hitboxMultiplier;
        }

        private void OnTriggerEnter2D(Collider2D collider2d)
        {
            if (!collider2d.TryGetComponent(out SlimeCharacter character)) return;
            var dispatcher = ServiceLocator.Instance.GetService<EventDispatcher>();
            dispatcher.Trigger<CoinCollectedEvent>(_coinCollectedEvent);
            Destroy(gameObject);
        }
    }
}