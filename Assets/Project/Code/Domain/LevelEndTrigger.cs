using NaughtyAttributes;
using Project.Code.Domain.Events;
using Project.Code.Patterns.Events;
using Project.Code.Patterns.Services;
using UnityEngine;

namespace Project.Code.Domain
{
    [SelectionBase]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class LevelEndTrigger : MonoBehaviour
    {
        [SerializeField, Scene] private string sceneToSwitch;

        private void Reset()
        {
            // var col = GetComponent<BoxCollider2D>();
            var rb = GetComponent<BoxCollider2D>();
            
            rb.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D collider2d)
        {
            if (!collider2d.TryGetComponent(out SlimeCharacter _)) return;

            Debug.Break();

            // desabilitarse a sí mismo, o lo que sea xd
            this.enabled = false;
            
            // lo importante es esto, avisar que se ha producido un evento de fin de nivel
            var dispatcher = ServiceLocator.Instance.GetService<EventDispatcher>();
            var levelWinConditionMetEvent = new LevelWinConditionMetEvent { SceneToSwitch = sceneToSwitch };

            dispatcher.Trigger<LevelWinConditionMetEvent>(levelWinConditionMetEvent);
        }
    }
}