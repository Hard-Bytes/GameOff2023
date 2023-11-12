using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Code.UI
{
    public class HPBarInstaller : MonoBehaviour
    {
        [SerializeField] private HPBarView hpBarView;
        [SerializeField] private HPBarPresenter hPBarPresenter;
        private List<IDisposable> _disposables;

        private void Awake()
        {
            hPBarPresenter.Configure(hpBarView);

        }
    }
}