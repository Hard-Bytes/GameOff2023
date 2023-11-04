using UnityEngine;
using UnityEngine.UI;

namespace Project.Code.UI.MainMenu
{
    public class MainMenuView : ViewBase
    {
        private MainMenuViewModel _viewModel;
        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;

        public void Configure(MainMenuViewModel viewModel)
        {
            _viewModel = viewModel;
            
            playButton.onClick.AddListener(OnPlayButtonPressed);
            quitButton.onClick.AddListener(OnQuitButtonPressed);
        }

        private void OnPlayButtonPressed() => _viewModel.playButtonPressed.Execute();
        private void OnQuitButtonPressed() => _viewModel.quitButtonPressed.Execute();

        private void OnDestroy()
        {
            playButton.onClick.RemoveAllListeners();
            quitButton.onClick.RemoveAllListeners();
        }
    }
}