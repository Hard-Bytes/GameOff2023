using UniRx;

namespace Project.Code.UI.HPBar
{
    public class HPBarViewModel
    {
        // Reactive Properties
        public readonly ReactiveProperty<int> CurrentHP;
        public readonly ReactiveProperty<int> DivideAmount;
        public readonly ReactiveProperty<int> MaxHealth;

        // Reactive Commands

        public HPBarViewModel()
        {
            CurrentHP = new ReactiveProperty<int>(0);
            DivideAmount = new ReactiveProperty<int>(5);
            MaxHealth = new ReactiveProperty<int>(10);
        }
    }
}