using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Project.Code.Domain.Events;
using Project.Code.Patterns.Events;
using Project.Code.Patterns.Services;

namespace Project.Code.Utils
{
    [System.Serializable]
    public class HealthComponent
    {
        [Header("HP Parameters")]
        [SerializeField] int startHP;
        [SerializeField] int maxHP;
        public int healthPoints;
        [SerializeField] int ThresHoldSmallMedium;
        [SerializeField] int ThresHoldMediumBig;

        public readonly ReactiveProperty<SlimeSize> Size;

        public HealthComponent()
        {
            Size = new ReactiveProperty<SlimeSize>(SlimeSize.Small);
        }

        public void Initialize()
        {
            // Subscribirse a los eventos de input
            healthPoints = startHP;
            UpdateSize();
            CommunicateHPChange();
        }

        public void ChangeHP(int change)
        {
            healthPoints += change;
            if (healthPoints > maxHP) healthPoints = maxHP;
            UpdateSize();
            CommunicateHPChange();
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
            CommunicateHPChange();
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

        private void CommunicateHPChange()
        {

            var dispatcher = ServiceLocator.Instance.GetService<EventDispatcher>();
            var signal = new CharacterHPChangeEvent { NuevaHP = healthPoints, NuevaDivision = 10 , VidaMaxima = maxHP};
            dispatcher.Trigger<CharacterHPChangeEvent>(signal);
        }
    }
}