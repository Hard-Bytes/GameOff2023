using System;
using UnityEngine;
using Project.Code.Utils;

namespace Project.Code.Domain
{
    [SelectionBase]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class SlimeEnemy : MonoBehaviour
    {

        [Header("Values")]
        [SerializeField] private float speed = 3.0f;
        [SerializeField] private int damage = 3;
        [SerializeField] private int slimeDrop = 3;
        [SerializeField] private SlimeSize size = SlimeSize.Small;
        [Header("Points movement")]
        [SerializeField] private Vector3 Objective1 = new Vector3(0, 0, 0);
        [SerializeField] private Vector3 Objective2 = new Vector3(0, 0, 0);
        [Header("SoldierSlime Parameters")]
        [SerializeField] private bool helmet = false;
        [SerializeField] private float timeOfStun = 1.0f;
        private float stunedTime = 0;

        private Vector3 positionObjective1 = new Vector3(0, 0, 0);
        private Vector3 positionObjective2 = new Vector3(0, 0, 0);
        private Vector3 ActualObjective = new Vector3(0, 0, 0);


        private Vector2 movementDirection;
        private Transform transform;
        private Rigidbody2D rigidbody;
        private BoxCollider2D collider;

        private void Awake()
        {
            transform = GetComponent<Transform>();
            rigidbody = GetComponent<Rigidbody2D>();
            collider = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            positionObjective1 = transform.position + Objective1;
            positionObjective2 = transform.position + Objective2;
            movementDirection = positionObjective1 - transform.position;
            movementDirection.Normalize();
            ActualObjective = positionObjective1;
        }

        private void FixedUpdate()
        {
            if(stunedTime <= 0)
            {
                MoveEntity();
                ChangeObjective();
            }
            else
            {
                stunedTime -= Time.fixedDeltaTime;
            }
        }

        private void MoveEntity()
        {
            Vector3 deltaMovement = movementDirection * (speed * Time.fixedDeltaTime);
            transform.position += deltaMovement;
        }

        private void LoseHelmet()
        {
            stunedTime = 0.3f;
        }

        private void ChangeObjective()
        {
            if(Vector3.Distance(transform.position, ActualObjective)<0.1f)
            {
                if(ActualObjective == positionObjective2)
                {
                    ActualObjective = positionObjective1;
                    movementDirection = positionObjective1 - transform.position;
                    movementDirection.Normalize();
                }
                else if (ActualObjective == positionObjective1)
                {
                    ActualObjective = positionObjective2;
                    movementDirection = positionObjective2 - transform.position;
                    movementDirection.Normalize();
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.gameObject.TryGetComponent<SlimeCharacter>(out SlimeCharacter player))
            {
                ContactPoint2D contact = collision.contacts[0];
                if(transform.position.y+ collider.size.y/2*0.9< contact.point.y)
                {
                    if(!helmet && size <= player.GetSize())
                    {
                        player.ChangeHP(slimeDrop, DamageSource.Enemy);
                        Destroy(gameObject);
                    }
                    else
                    {
                        player.Bounce();
                    }
                }
                else
                {
                    player.ChangeHP(-damage, DamageSource.Enemy);
                }
            }
        }
    }
}
