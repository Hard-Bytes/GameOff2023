using Project.Code.Domain.Events;
using Project.Code.Patterns.Events;
using Project.Code.Patterns.Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Code.Domain.Managers
{
    public class GameController : MonoBehaviour
    {
        private void Start()
        {
            var dispatcher = ServiceLocator.Instance.GetService<EventDispatcher>();
            dispatcher.Subscribe<LevelWinConditionMetEvent>(OnLevelWinConditionMet);
        }

        private void OnLevelWinConditionMet(GameEvent evt)
        {
            var eventData = (LevelWinConditionMetEvent)evt;
            var nextScene = eventData.SceneToSwitch;

            // TODO: debería de llamar a nuestro SceneSwitchService, pero por ahora que cambie de escena a pelo
            SceneManager.LoadScene(nextScene);
        }

        private void OnDestroy()
        {
            var dispatcher = ServiceLocator.Instance.GetService<EventDispatcher>();
            dispatcher.Unsubscribe<LevelWinConditionMetEvent>(OnLevelWinConditionMet);
        }
    }
}