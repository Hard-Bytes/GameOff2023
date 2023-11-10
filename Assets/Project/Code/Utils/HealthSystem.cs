using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Project.Code.Utils
{
    [System.Serializable]
    public class HealthSystem
    {
        [Header("HP Parameters")]
        [SerializeField] int startHP;
        [SerializeField] int maxHP;
        public int healthPoints;
        [SerializeField] int ThresHoldSmallMedium;
        [SerializeField] int ThresHoldMediumBig;

        public readonly ReactiveProperty<SlimeSize> Size;

        public HealthSystem()
        {
            Size = new ReactiveProperty<SlimeSize>(SlimeSize.Small);
        }

        public void Initialize()
        {
            // Subscribirse a los eventos de input
            healthPoints = startHP;
            UpdateSize();
        }

        public void ChangeHP(int change)
        {
            healthPoints += change;
            if (healthPoints > maxHP) healthPoints = maxHP;
            UpdateSize();
        }

        public void SetHPFromSize(SlimeSize newSize)
        {
            switch(newSize)
            {
                case(SlimeSize.Big): healthPoints = maxHP; break;
                case(SlimeSize.Medium): healthPoints = ThresHoldMediumBig; break;
                case(SlimeSize.Small): healthPoints = ThresHoldSmallMedium; break;
            }
            UpdateSize();
        }

        private void UpdateSize()
        {
            var newSize = GetSize();
            if(Size.Value != newSize)
            {
                Size.Value = newSize;
            }
        }

        public SlimeSize GetSize()
        {
            if(healthPoints > ThresHoldMediumBig)
            {
                return SlimeSize.Big;
            }
            if(healthPoints > ThresHoldSmallMedium)
            {
                return SlimeSize.Medium;
            }
            return SlimeSize.Small;
        }

        public int GetMaxHP()
        {
            return maxHP;
        }

        public int GetHealthPoints()
        {
            return healthPoints;
        }
    }
}