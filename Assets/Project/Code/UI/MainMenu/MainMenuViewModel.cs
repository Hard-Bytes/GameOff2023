using UniRx;

namespace Project.Code.UI.MainMenu
{
    public class MainMenuViewModel
    {
        // Reactive Properties
        
        // Reactive Commands
        public readonly ReactiveCommand playButtonPressed;
        public readonly ReactiveCommand quitButtonPressed;

        public MainMenuViewModel()
        {
            playButtonPressed = new ReactiveCommand();
            quitButtonPressed = new ReactiveCommand();
        }
    }
}