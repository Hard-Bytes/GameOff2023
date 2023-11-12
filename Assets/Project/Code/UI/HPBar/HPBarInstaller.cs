using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Code.UI.HPBar
{
    public class HPBarInstaller : MonoBehaviour
    {
        [SerializeField] private HPBarView hpBarView;
        private List<IDisposable> _disposables;

        private void Awake()
        {
            HPBarViewModel viewModel = new HPBarViewModel();
            hpBarView.Configure(viewModel);
            var hPBarPresenter = new HPBarPresenter(viewModel);
        }
    }
}
