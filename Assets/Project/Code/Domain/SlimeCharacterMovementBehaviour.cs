using System;
using UnityEngine;

namespace Project.Code.Domain
{
    public abstract class SlimeCharacterMovementBehaviour : MonoBehaviour
    {
        protected bool IsRunning;

        public void SetRunning(bool running) => IsRunning = running;

        public abstract void UpdateMovement(Vector2 direction);
        
        [Obsolete]
        [Tooltip("Usar mejor la clase de SlimeCharacterJumpBehaviour dentro de SlimeCharacter")]
        public abstract void Jump();
    }
}