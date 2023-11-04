using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Code.UI.MainMenu
{
    public class MainMenuController : IDisposable
    {
        private readonly MainMenuViewModel _viewModel;
        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        private readonly string _sceneToLoad;

        public MainMenuController(MainMenuViewModel viewModel, string sceneToLoad)
        {
            _sceneToLoad = sceneToLoad;
            _viewModel = viewModel;

            _viewModel
                .playButtonPressed
                .Subscribe(OnPlayButtonPressed)
                .AddTo(_disposables);

            _viewModel
                .quitButtonPressed
                .Subscribe(OnQuitButtonPressed)
                .AddTo(_disposables);

            // _viewModel
            //     .quitButtonPressed
            //     .Subscribe(OnGameButtonPressed)
            //     .AddTo(_disposables);
        }
        
        private void OnPlayButtonPressed(Unit _) => SceneManager.LoadScene(_sceneToLoad);

        private void OnQuitButtonPressed(Unit _)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
        // private void OnGameButtonPressed(Unit _) => SceneManager.LoadScene("Main");

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}