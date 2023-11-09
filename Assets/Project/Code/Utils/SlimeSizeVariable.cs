using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Code.Utils
{
    [System.Serializable]
    public struct SlimeSizeVariable
    {
        [Header("Movements Parameters")]
        [SerializeField] float acceleration;
        [SerializeField] float maxVelocity;
        [SerializeField] float deceleration;
        [SerializeField] float jumpForce;
        [SerializeField] float bounceForce;

        [Header("Physics Parameters")]
        [SerializeField] float gravity;
        [SerializeField] float smashAttackVelocity;

    }
}