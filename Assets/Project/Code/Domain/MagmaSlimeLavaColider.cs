using System;
using System.Collections.Generic;
using UnityEngine;
using Project.Code.Utils;

namespace Project.Code.Domain
{
    public class MagmaSlimeLavaColider : MonoBehaviour
    {
        
        [SerializeField] private int damage = 3; 

        public void SetDamage(int newDmg)
        {
            damage = newDmg;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject.TryGetComponent<SlimeCharacter>(out SlimeCharacter player))
            {
                BoxCollider2D playerCollider = player.gameObject.GetComponent<BoxCollider2D>();
                player.ChangeHP(-damage, DamageSource.Enemy);
                player.Knockback(transform.position);
            }
        }
    }
}
