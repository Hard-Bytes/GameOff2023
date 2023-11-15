using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Code.Domain.Events;
using Project.Code.Patterns.Events;
using Project.Code.Patterns.Services;
using UniRx;

namespace Project.Code.UI.Coin
{
    public class CoinPresenter : IDisposable
    {
        private CoinViewModel _viewModel;
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        public CoinPresenter(CoinViewModel newViewModel)
        {
            _viewModel = newViewModel;

            var dispatcher = ServiceLocator.Instance.GetService<EventDispatcher>();
            dispatcher.Subscribe<CoinCollectedEvent>(CoinCollected);
        }

        private void CoinCollected(GameEvent evt)
        {
            var eventdata = (CoinCollectedEvent)evt;
            _viewModel.Points.Value = 0;
            _viewModel.Points.Value = eventdata.points;
            _viewModel.isBigCoin.Value = eventdata.isBigCoin;
            
        }

        public void Dispose()
        {
            var dispatcher = ServiceLocator.Instance.GetService<EventDispatcher>();
            dispatcher.Unsubscribe<CoinCollectedEvent>(CoinCollected);

            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }

}