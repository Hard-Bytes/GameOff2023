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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log("Colisiona Conmigo Papa");
            if(collision.gameObject.TryGetComponent<SlimeCharacter>(out SlimeCharacter player))
            {
                BoxCollider2D playerCollider = player.gameObject.GetComponent<BoxCollider2D>();
                player.ChangeHP(-damage, DamageSource.Enemy);
                player.Knockback(transform.position);
            }
        }
    }
}
