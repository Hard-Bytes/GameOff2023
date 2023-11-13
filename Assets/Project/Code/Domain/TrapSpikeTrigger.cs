using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Project.Code.Utils;
using Project.Level.Scriptables;
using Unity.VisualScripting;
using UnityEngine;

namespace Project.Code.Domain
{
    public interface Trap { }
    
    [RequireComponent(typeof(BoxCollider2D))]
    public class TrapSpikeTrigger : MonoBehaviour, Trap
    {
        [SerializeField] private int slimeDamage;
        [SerializeField] private float pushForce;
        [SerializeField] private float verticalPushForceModifier = 1f;
        [SerializeField,Range(0,360)] private float pushAngle;
        [SerializeField] private float invulnerabilityTime = -1;
        [SerializeField] private bool ignoreInvulnerability = false;
        [HideInInspector] BoxCollider2D _boxCollider;
        [SerializeField] private DamageSource type = DamageSource.SpikeTrap;
        [SerializeField] private TrapScriptableObject preset = null;
        public void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
            if (preset != null)
            {
                slimeDamage = preset.slimeDamage;
                pushForce = preset.pushForce;
                verticalPushForceModifier = preset.verticalPushForceModifier;
                pushAngle = preset.pushAngle;
                invulnerabilityTime = preset.invulnerabilityTime;
                ignoreInvulnerability = preset.ignoreInvulnerability;
                type = preset.type;
            }
        }

        private void OnTriggerEnter2D(Collider2D collider2d)
        {
            if (!collider2d.TryGetComponent(out SlimeCharacter character)) return;
            
            // get the positions to calculate knock back direction
            Vector2 playerPos = collider2d.gameObject.transform.position;
            var contactPoint = _boxCollider.ClosestPoint(playerPos);
            var directionVector = contactPoint - playerPos;
            float dir = MathF.Abs(MathF.Atan2(directionVector.x, directionVector.y)* Mathf.Rad2Deg);
            
            //
            int directionX = playerPos.x > contactPoint.x ? 1 : -1;
            int directionY = playerPos.y > contactPoint.y ? 1 : -1;
            float knockbackStrength = pushForce * (dir > 90 ? verticalPushForceModifier : 1f);
            Vector2 movement = new Vector2(Mathf.Cos(pushAngle* Mathf.Deg2Rad) * directionX, Mathf.Sin(pushAngle* Mathf.Deg2Rad) * directionY) * knockbackStrength;
            
            character.TrapKnockBack(movement, slimeDamage, type, invulnerabilityTime, ignoreInvulnerability);
        }
    }
}