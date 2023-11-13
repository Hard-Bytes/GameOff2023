using Project.Code.Utils;
using UnityEngine;

namespace Project.Level.Scriptables
{
    [CreateAssetMenu(fileName="Trap",menuName="ScriptableObjects/Traps")]
    public class TrapScriptableObject : ScriptableObject
    {
        [SerializeField] public int slimeDamage;
        [SerializeField] public float pushForce;
        [SerializeField] public float verticalPushForceModifier = 1f;
        [SerializeField,Range(0,360)] public float pushAngle;
        [SerializeField] public float invulnerabilityTime = -1f;
        [SerializeField] public bool ignoreInvulnerability = false;
        [SerializeField] public DamageSource type = DamageSource.SpikeTrap;
    }
}
