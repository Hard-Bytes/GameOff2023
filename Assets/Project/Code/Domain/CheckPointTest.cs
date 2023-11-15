using System;
using Project.Code.Domain.Events;
using Project.Code.Patterns.Events;
using Project.Code.Patterns.Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Code.Domain
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class CheckPointTest : MonoBehaviour
    {
        [SerializeField] private string sceneToLoad;
        private bool _passed = false;

        private void Awake()
        {
            var dispatcher = ServiceLocator.Instance.GetService<EventDispatcher>();
            dispatcher.Subscribe<CharacterDiedEvent>(OnCharacterDeath);
        }

        private void OnCharacterDeath(GameEvent obj)
        {
            SceneManager.UnloadSceneAsync(sceneToLoad);
            SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
        }

        private void OnTriggerEnter2D(Collider2D collider2d)
        {
            if (!_passed && !collider2d.TryGetComponent(out SlimeCharacter character)) return;
            SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
            _passed = true;
        }
    }
}