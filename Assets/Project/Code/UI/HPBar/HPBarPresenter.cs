using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Code.Domain.Events;
using Project.Code.Patterns.Events;
using Project.Code.Patterns.Services;
using UniRx;

namespace Project.Code.UI.HPBar
{
    public class HPBarPresenter : IDisposable
    {
        private HPBarViewModel _viewModel;
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        public HPBarPresenter(HPBarViewModel newViewModel)
        {
            _viewModel = newViewModel;

            var dispatcher = ServiceLocator.Instance.GetService<EventDispatcher>();
            dispatcher.Subscribe<CharacterHPChangeEvent>(OnChangeHP);
        }

        private void OnChangeHP(GameEvent evt)
        {
            var eventdata = (CharacterHPChangeEvent)evt;
            if(_viewModel.CurrentHP.Value != eventdata.NewHP)
            {
                _viewModel.CurrentHP.Value = eventdata.NewHP;
            }
            if(_viewModel.DivideAmount.Value != eventdata.NewDivision)
            {
                _viewModel.DivideAmount.Value = eventdata.NewDivision;
            }
            if(_viewModel.MaxHealth.Value != eventdata.MaxHP)
            {
                _viewModel.MaxHealth.Value = eventdata.MaxHP;
            }
        }

        public void Dispose()
        {
            var dispatcher = ServiceLocator.Instance.GetService<EventDispatcher>();
            dispatcher.Unsubscribe<CharacterHPChangeEvent>(OnChangeHP);

            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }

}