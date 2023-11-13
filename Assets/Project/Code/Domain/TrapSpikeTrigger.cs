using System;
using NaughtyAttributes;
using Project.Code.Utils;
using UnityEngine;

namespace Project.Code.Domain
{
    public interface Trap { }
    
    [RequireComponent(typeof(BoxCollider2D))]
    public class TrapSpikeTrigger : MonoBehaviour, Trap
    {
        [SerializeField] private int damage;
        [SerializeField] private float pushForce;
        [SerializeField,Range(0,360)] private float pushAngle;
        [SerializeField] private float invulnerabilityTime;

        private void OnTriggerEnter2D(Collider2D collider2d)
        {
            
        }
    }
}