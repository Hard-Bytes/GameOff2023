using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Project.Code.UI.HPBar
{
    public class HPBarView : ViewBase
    {
        private HPBarViewModel _viewModel;
        [SerializeField] private GameObject healthBar;
        [SerializeField] private GameObject divideBar;
        [SerializeField] private float scaleSpeed=0.05f;
        private float _percentage;
        private float _percentageDivide;

        public void Configure(HPBarViewModel viewModel)
        {
            _viewModel = viewModel;

            _viewModel.CurrentHP.Subscribe(OnParamChanged).AddTo(_disposables);
            _viewModel.DivideAmount.Subscribe(OnParamChanged).AddTo(_disposables);
            _viewModel.MaxHealth.Subscribe(OnParamChanged).AddTo(_disposables);
        }

        void Update()
        {
            float barSize = healthBar.transform.localScale.x;
            if(Mathf.Abs(_percentage - (1-barSize)) > 0.01f)
            {
                float changebar = (_percentage > 1 - barSize ?-scaleSpeed : scaleSpeed) *Time.fixedDeltaTime;
                healthBar.transform.localScale = healthBar.transform.localScale + new Vector3(changebar,0,0);
            }
            float barDivideSize = divideBar.transform.localScale.x;
            if(Mathf.Abs(_percentage - _percentageDivide - (1- barDivideSize)) > 0.01f)
            {
                float changebar = (_percentage > 1 - barSize ?-scaleSpeed : scaleSpeed) *Time.fixedDeltaTime;
                divideBar.transform.localScale = divideBar.transform.localScale + new Vector3(changebar,0,0);
            }
        }

        private void OnParamChanged(int _) { RecalculatePercentage(); }

        public void RecalculatePercentage()
        {
            _percentage = (float)_viewModel.CurrentHP.Value / (float)_viewModel.MaxHealth.Value;
            _percentageDivide = (float)_viewModel.DivideAmount.Value / (float)_viewModel.MaxHealth.Value;
        }
    }
}
