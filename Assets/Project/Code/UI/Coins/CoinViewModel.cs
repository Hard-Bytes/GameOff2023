using UniRx;

namespace Project.Code.UI.Coin
{
    public class CoinViewModel
    {
        // Reactive Properties
        public readonly ReactiveProperty<int> Points;
        public readonly ReactiveProperty<bool> isBigCoin;

        // Reactive Commands

        public CoinViewModel()
        {
            Points = new ReactiveProperty<int>(0);
            isBigCoin = new ReactiveProperty<bool>(false);
        }
    }
}