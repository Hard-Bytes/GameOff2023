using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Project.Code.UI.MainMenu
{
    public class MainMenuInstaller : MonoBehaviour
    {
        [SerializeField] private MainMenuView mainMenuView;
        [SerializeField, Scene] private string sceneToLoad;
        private List<IDisposable> _disposables;
        
        private void Awake()
        {
            MainMenuViewModel viewModel = new MainMenuViewModel();
            mainMenuView.Configure(viewModel);
            var mainMenuController = new MainMenuController(viewModel, sceneToLoad);
        }
    }
}