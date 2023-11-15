using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using TMPro;


namespace Project.Code.UI.Coin
{
    public class CoinView : ViewBase
    {
        private CoinViewModel _viewModel;
        [SerializeField] private TMP_Text coins;
        private int totalCoins;

        public void Configure(CoinViewModel viewModel)
        {
            _viewModel = viewModel;

            _viewModel.Points.Subscribe(OnParamChanged).AddTo(_disposables);
        }

        void Start()
        {
            totalCoins = 0;
        }

        private void OnParamChanged(int _) { RecalculateCoins(); }

        public void RecalculateCoins()
        {
            Debug.Log("nueva moneda");
            totalCoins += _viewModel.Points.Value;
            coins.text = totalCoins.ToString();
        }
    }
}
