using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Code.Domain.Events;
using Project.Code.Patterns.Events;
using Project.Code.Patterns.Services;

namespace Project.Code.UI
{
    public class HPBarPresenter : MonoBehaviour
    {
        private HPBarView _view;
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        public void Configure(HPBarView newview)
        {
            _view = newview;
        }
        // Start is called before the first frame update
        void Start()
        {
            var dispatcher = ServiceLocator.Instance.GetService<EventDispatcher>();
            dispatcher.Subscribe<CharacterHPChangeEvent>(OnChangeHP);

        }

        // Update is called once per frame
        void OnDestroy()
        {
            var dispatcher = ServiceLocator.Instance.GetService<EventDispatcher>();
            dispatcher.Unsubscribe<CharacterHPChangeEvent>(OnChangeHP);

        }

        private void OnChangeHP(GameEvent evt)
        {
            var eventdata = (CharacterHPChangeEvent)evt;
            _view.RecalculatePercentage(eventdata.NuevaHP, eventdata.NuevaDivision, eventdata.VidaMaxima);
        }


        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }

}