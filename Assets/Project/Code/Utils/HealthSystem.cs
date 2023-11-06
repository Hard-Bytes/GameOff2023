using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Code.Utils
{
    [System.Serializable]
    public struct HealthSystem
    {
        [Header("HP Parameters")]
        [SerializeField] int maxHP;
        private int healthPoints;
        [SerializeField] int ThresHoldSmallMedium;
        [SerializeField] int ThresHoldMediumBig;

        public Size changeHP(int change)
        {
            healthPoints += change;
            if (healthPoints > maxHP) healthPoints = maxHP;
            return CheckSize();
        }

        public Size CheckSize()
        {
            if(healthPoints > ThresHoldMediumBig)
            {
                return Size.Big;
            }
            if(healthPoints > ThresHoldSmallMedium)
            {
                return Size.Medium;
            }
            return Size.Small;
        }
    }
}