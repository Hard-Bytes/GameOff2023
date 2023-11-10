using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Code.Domain
{
    public class SlimeCharacterInputReceiver : MonoBehaviour, SlimeCharacterInput.IPlayerActions
    {
        private SlimeCharacterInput _slimeCharacterInput;

        public event Action OnJumpActionStart;
        public event Action OnJumpActionEnd;
        public event Action OnRunActionStart;
        public event Action OnRunActionEnd;
        public event Action<Vector2> OnMovementAction;

        private void Awake()
        {
            _slimeCharacterInput = new SlimeCharacterInput();
            _slimeCharacterInput.Player.SetCallbacks(this);
            _slimeCharacterInput.Player.Enable();
        }

        private void OnDestroy()
        {
            _slimeCharacterInput.Player.Disable();
            _slimeCharacterInput.Player.SetCallbacks(null);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();
            OnMovementAction?.Invoke(value);
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            var phase = context.phase;

            if (phase == InputActionPhase.Started)
            {
                OnRunActionStart?.Invoke();
            }
            else if (phase == InputActionPhase.Canceled)
            {
                OnRunActionEnd?.Invoke();
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            
            var phase = context.phase;

            if (phase == InputActionPhase.Started)
            {
                OnJumpActionStart?.Invoke();
            } 
            else if (phase == InputActionPhase.Canceled)
            {
                OnJumpActionEnd?.Invoke();
            }
        }

        public void OnMelee(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }
    }
}