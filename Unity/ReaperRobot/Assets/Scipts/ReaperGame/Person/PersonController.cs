using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

namespace smart3tene.Reaper
{
    [RequireComponent(typeof(PlayerInput))]
    public class PersonController : MonoBehaviour
    {
        #region private Fields
        private PersonManager _personManager;
        private InputActionMap _personAction;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            _personManager = GameSystem.Instance.PersonInstance.GetComponent<PersonManager>();
            _personAction = GetComponent<PlayerInput>().actions.FindActionMap("Person");

            _personAction["ChangeMode"].started += ChangeViewMode;
            _personAction["CloseApp"].started += CloseApp;
            _personAction["Menu"].started += InvokeMenuEvent;

            if (GameSystem.Instance == null) return;
            GameSystem.Instance.NowViewMode.Subscribe(mode =>
            {
                switch (mode)
                {
                    case GameSystem.ViewMode.PERSON_TPV:
                        GetComponent<PlayerInput>().SwitchCurrentActionMap("Person");
                        break;
                    default:
                        break;
                }
            });
        }

        private void OnDisable()
        {
            _personAction["ChangeMode"].started -= ChangeViewMode;
            _personAction["CloseApp"].started -= CloseApp;
            _personAction["Menu"].started -= InvokeMenuEvent;
        }

        private void LateUpdate()
        {
            var move = _personAction["Look"].ReadValue<Vector2>();
            _personManager.RotateTPVCamera(move.x, move.y);
        }

        private void FixedUpdate()
        {
            var move = _personAction["Move"].ReadValue<Vector2>();
            _personManager.Move(move.x, move.y);
        }
        #endregion

        #region Private Fields
        private void ChangeViewMode(InputAction.CallbackContext obj)
        {
            if (GameSystem.Instance != null)
            {
                GameSystem.Instance.ChangeViewMode();
            }
        }


        private void CloseApp(InputAction.CallbackContext obj)
        {
            //SceneTransitionManagerがシーンにないとCloseAppできません
            if (SceneTransitionManager.Instantiated)
            {
                SceneTransitionManager.Instance.CloseApp();
            }
        }
        private void InvokeMenuEvent(InputAction.CallbackContext obj)
        {
            GameSystem.Instance.InvokeMenuEvent();
        }
        #endregion
    }
}