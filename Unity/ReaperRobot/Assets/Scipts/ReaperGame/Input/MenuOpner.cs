using UnityEngine;
using UnityEngine.InputSystem;

namespace smart3tene.Reaper
{
    [RequireComponent(typeof(PlayerInput))]
    public class MenuOpner : MonoBehaviour
    {
        #region Private Fields
        private InputActionMap _reaperActionMap;
        private InputActionMap _personActionMap;

        private PlayerInput _playerInput;
        private bool _isMenuOpen = false;
        private bool _wasUseTimer = false;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _playerInput.enabled = true;

            _reaperActionMap = _playerInput.actions.FindActionMap("Reaper");
            _personActionMap = _playerInput.actions.FindActionMap("Person");

            _reaperActionMap["Menu"].started += MenuAction;
            _personActionMap["Menu"].started += MenuAction;

            ReaperEventManager.OpenMenuEvent += OnOpenMenuEvent;
            ReaperEventManager.CloseMenuEvent += OnCloseMenuEvent;
        }

        private void OnDestroy()
        {
            _reaperActionMap["Menu"].started -= MenuAction;
            _personActionMap["Menu"].started -= MenuAction;

            ReaperEventManager.OpenMenuEvent -= OnOpenMenuEvent;
            ReaperEventManager.CloseMenuEvent -= OnCloseMenuEvent;

            Time.timeScale = 1;
        }
        #endregion

        #region Private method
        private void MenuAction(InputAction.CallbackContext obj)
        {
            if (_isMenuOpen)
            {
                ReaperEventManager.InvokeCloseMenuEvent();
            }
            else
            {
                ReaperEventManager.InvokeOpenMenuEvent();
            }
        }

        private void OnOpenMenuEvent()
        {
            _isMenuOpen = true;

            _wasUseTimer = GameTimer.IsTimerRunning;
            if (GameTimer.IsTimerRunning) GameTimer.Stop();

            Time.timeScale = 0;

            _playerInput.enabled = false;
        }

        private void OnCloseMenuEvent()
        {
            _isMenuOpen = false;

            if(_wasUseTimer) GameTimer.Start();

            Time.timeScale = 1;

            _playerInput.enabled = true;
        }
        #endregion
    }

}
