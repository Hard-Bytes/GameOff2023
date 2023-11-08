using UnityEngine;

namespace Project.Code.Domain
{
    // Esta es la forma en la que se hacía antes
    public class SimpleSlimeCharacterMovement : SlimeCharacterMovementBehaviour
    {
        [Header("--> Referencias")]
        [SerializeField] private new Transform transform;
        [SerializeField] private new Rigidbody2D rigidbody;
        
        [Header("--> Valores")] 
        [SerializeField] private float speedWalking = 3.0f;
        [SerializeField] private float speedRunning = 6.0f;

        [Tooltip("Se usará esto en vez del jumpforce, pero por ahora bien")]
        [SerializeField] private float jumpHeight = 2.0f;
        [SerializeField] private float jumpForce = 300.0f;

        private void Reset()
        {
            transform = GetComponent<Transform>(); 
            rigidbody = GetComponent<Rigidbody2D>(); 
        }

        public override void UpdateMovement(Vector2 direction)
        {
            float speed = IsRunning ? speedRunning : speedWalking;
            Vector3 deltaMovement = direction * (speed * Time.fixedDeltaTime);
            transform.position += deltaMovement;
        }

        public override void Jump()
        {
            rigidbody.AddForce(Vector2.up * jumpForce);
        }
    }
}