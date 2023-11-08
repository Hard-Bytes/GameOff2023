using UnityEngine;

namespace Project.Code.Domain
{
    public abstract class SlimeCharacterMovementBehaviour : MonoBehaviour
    {
        protected bool IsRunning;

        public void SetRunning(bool running) => IsRunning = running;

        public abstract void UpdateMovement(Vector2 direction);
        public abstract void Jump();
    }
}