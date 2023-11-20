using System;
using UnityEngine;
using Project.Code.Utils;

namespace Project.Code.Domain
{
    [SelectionBase]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class MagmaSlimeEnemy : MonoBehaviour
    {

        [Header("Values")]
        [SerializeField] private float speed = 3.0f;
        [SerializeField] private int damage = 3;
        [SerializeField] private SlimeSize size = SlimeSize.Small;
        [SerializeField] private bool staticEnemy;
        [Header("Magma Slime Parameters")]
        [SerializeField] private int magmaDamage;
        [SerializeField] private float magmaLifeSpan;
        [SerializeField] private GameObject trail;

        [Header("Points movement")]
        [SerializeField] private GameObject Objective1;
        [SerializeField] private GameObject Objective2;
        [SerializeField] private float stopTimeWhenObjective;
        private float stunedTime = 0;

        private Vector3 positionObjective1 = new Vector3(0, 0, 0);
        private Vector3 positionObjective2 = new Vector3(0, 0, 0);
        private Vector3 ActualObjective = new Vector3(0, 0, 0);
        private Vector3 deltaMovement;


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

            trail.GetComponent<MagmaSlimeLava>().SetDamage(magmaDamage);
            trail.GetComponent<TrailRenderer>().time = magmaLifeSpan;
        }

        private void FixedUpdate()
        {
            if (!staticEnemy)
            {
                if (stunedTime <= 0)
                {
                    MoveEntity();
                    ChangeObjective();
                }
                else
                {
                    stunedTime -= Time.fixedDeltaTime;
                }
            }
        }

        private void MoveEntity()
        {
            deltaMovement = movementDirection * (speed * Time.fixedDeltaTime);
            transform.position += deltaMovement;
        }

        private void ChangeObjective()
        {
            if(Vector3.Distance(new Vector3(transform.position.x, 0, 0), ActualObjective)<0.1f)
            {
                stunedTime = stopTimeWhenObjective;
                if (ActualObjective == positionObjective2)
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
                player.ChangeHP(-damage, DamageSource.Enemy);
                player.Knockback(transform.position);
            }
        }
    }
}
