using UnityEngine;

namespace Project.Code.Domain
{
    public abstract class SlimeCharacterJumpBehaviour : MonoBehaviour
    {
        public abstract void DoJump();
        public abstract void CancelJump();
        public abstract bool OnGround();
    }
}