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
        [SerializeField, Range(0f, 1f)] private float thresholdJump = 0.9f;
        [Header("Points movement")]
        [SerializeField] private GameObject Objective1;
        [SerializeField] private GameObject Objective2;
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
            positionObjective1 = new Vector3 ( Objective1.transform.position.x, 0, 0 );
            positionObjective2 = new Vector3 ( Objective2.transform.position.x, 0, 0 );
            movementDirection = positionObjective1 - new Vector3(transform.position.x, 0, 0); 
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
            if(Vector3.Distance(new Vector3(transform.position.x, 0, 0), ActualObjective)<0.1f)
            {
                if(ActualObjective == positionObjective2)
                {
                    ActualObjective = positionObjective1;
                    movementDirection = positionObjective1 - new Vector3(transform.position.x, 0, 0);
                    movementDirection.Normalize();
                }
                else if (ActualObjective == positionObjective1)
                {
                    ActualObjective = positionObjective2;
                    movementDirection = positionObjective2 - new Vector3(transform.position.x, 0, 0);
                    movementDirection.Normalize();
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.gameObject.TryGetComponent<SlimeCharacter>(out SlimeCharacter player))
            {
                BoxCollider2D playerCollider = player.gameObject.GetComponent<BoxCollider2D>();
                if (transform.position.y+ collider.size.y/2* thresholdJump <= player.transform.position.y + playerCollider.size.y/2*0.9)
                {
                    if (!helmet && size <= player.GetSize())
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
                    player.Knockback(transform.position);
                }
            }
        }
    }
}
