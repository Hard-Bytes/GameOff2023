using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Code.UI.Coin
{
    public class CoinInstaller : MonoBehaviour
    {
        [SerializeField] private CoinView CoinView;
        private List<IDisposable> _disposables;

        private void Awake()
        {
            CoinViewModel viewModel = new CoinViewModel();
            CoinView.Configure(viewModel);
            var CoinPresenter = new CoinPresenter(viewModel);
        }
    }
}
