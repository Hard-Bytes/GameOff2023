using System;
using UnityEngine;
using Project.Code.Utils;
using DG.Tweening;

namespace Project.Code.Domain
{
    [SelectionBase]
    [RequireComponent(typeof(CircleCollider2D))]
    public class SlimeScrap : MonoBehaviour
    {

        [Header("Values")]
        [SerializeField] private int slimeDrop = 10;
        [SerializeField] private bool orignatedByCharacter = false;

        [Header("Movement")]
        [SerializeField, Range(0.1f, 3f)] private float timeToDestinationSeconds = 1.0f;

        private bool _moving = false;

        private Transform _transform;
        private CircleCollider2D _trigger;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            _trigger = GetComponent<CircleCollider2D>();
        }

        public void SetOriginatedByCharacter(bool p_byChar)
        {
            orignatedByCharacter = p_byChar;
        }

        public void SetScrapValue(int p_value)
        {
            slimeDrop = p_value;
        }

        public void SetOffsetDestination(Vector2 p_newOffset)
        {
            if(_moving) return;

            Vector3 destination = _transform.position + new Vector3(p_newOffset.x, p_newOffset.y, 0);

            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                p_newOffset,
                p_newOffset.magnitude,
                //TODO If someone's got a better alternative than layer names, I'm all ears
                LayerMask.GetMask("Ground")
            );
            if (hit.collider != null)
            {
                float halfSize = _trigger.bounds.extents.x;
                if(p_newOffset.x < 0) halfSize *= -1;
                destination = hit.point - new Vector2(halfSize, 0);
            }

            _transform
                .DOMove(destination, timeToDestinationSeconds)
                .OnStart(delegate
                {
                    _moving = true;
                })
                .OnComplete(delegate
                {
                    _moving = false;
                })
                ;
        }

        private void OnTriggerEnter2D(Collider2D collider2d)
        {
            if(_moving) return;
            if (!collider2d.TryGetComponent(out SlimeCharacter character)) return;

            float diffY = collider2d.bounds.min.y - _trigger.bounds.center.y;
            if(!orignatedByCharacter || diffY > 0)
            {
                character.ChangeHP(slimeDrop, DamageSource.SlimeScrap, true);
                Destroy(gameObject);
            }
        }
    }
}
