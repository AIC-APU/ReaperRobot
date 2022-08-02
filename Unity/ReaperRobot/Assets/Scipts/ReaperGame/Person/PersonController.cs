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

        private bool _isOperatable = true;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            _personManager = GameSystem.Instance.PersonInstance.GetComponent<PersonManager>();

            _personAction = GetComponent<PlayerInput>().actions.FindActionMap("Person");

            _personAction["ChangeMode"].started += ChangeViewMode;
            _personAction["CloseApp"].started += CloseApp;
            _personAction["Menu"].started += InvokeMenuEvent;
            _personAction["ChangeReaperAndPerson"].started += ChangeReaperAndPerson;


            if (GameSystem.Instance == null) return;
            GameSystem.Instance.NowViewMode.Subscribe(mode =>
            {
                _personManager.StopMove();

                var playerInput = GetComponent<PlayerInput>();

                switch (mode)
                {
                    case GameSystem.ViewMode.PERSON_TPV:
                        if(playerInput.currentActionMap.name != "Person") playerInput.SwitchCurrentActionMap("Person");
                        _isOperatable = true;
                        break;
                    default:
                        _isOperatable = false;
                        break;
                }
            });
        }

        private void OnDisable()
        {
            _personAction["ChangeMode"].started -= ChangeViewMode;
            _personAction["CloseApp"].started -= CloseApp;
            _personAction["Menu"].started -= InvokeMenuEvent;
            _personAction["ChangeReaperAndPerson"].started -= ChangeReaperAndPerson;
        }

        private void LateUpdate()
        {
            if (GameSystem.Instance.NowViewMode.Value == GameSystem.ViewMode.REAPER_FromPERSON)
            {
                _personManager.FPVCameraFollow(GameSystem.Instance.ReaperInstance.transform);
            }

            if (!_isOperatable) return;

            var move = _personAction["Look"].ReadValue<Vector2>();
            _personManager.RotateTPVCamera(move.x, move.y);
            
            if (GameSystem.Instance.NowViewMode.Value == GameSystem.ViewMode.PERSON_TPV)
            {
                _personManager.TPVCameraFollow();
            }
        }

        private void FixedUpdate()
        {
            if (!_isOperatable) return;

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
        private void ChangeReaperAndPerson(InputAction.CallbackContext obj)
        {
            if (GameSystem.Instance != null)
            {
                GameSystem.Instance.ChangeReaperAndPerson();
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
            if(GameSystem.Instance != null)
            {
                GameSystem.Instance.InvokeMenuEvent();
            }
        }
        #endregion
    }
}