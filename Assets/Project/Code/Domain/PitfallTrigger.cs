using UnityEngine;
using Project.Code.Utils;

namespace Project.Code.Domain
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class PitfallTrigger : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collider2d)
        {
            if (!collider2d.TryGetComponent(out SlimeCharacter character)) return;
            character.Kill(DamageSource.PitfallTrap);
        }
    }
}
