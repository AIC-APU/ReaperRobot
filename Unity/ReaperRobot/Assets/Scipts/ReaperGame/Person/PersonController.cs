using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace smart3tene.Reaper
{
    [RequireComponent(typeof(PlayerInput))]
    public class PersonController : MonoBehaviour
    {
        #region private Fields
        [SerializeField, Tooltip("マルチプレイの時はnullにしておいてください")] private PersonManager _personManager;
        private InputActionMap _personAction;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            if(_personManager == null)
            {
                _personManager = GameSystem.Instance.PersonInstance.GetComponent<PersonManager>();
            }
            _personAction = GetComponent<PlayerInput>().actions.FindActionMap("Person");

            _personAction["ChangeMode"].started += ChangeViewMode;
        }

        private void OnDisable()
        {
            _personAction["ChangeMode"].started -= ChangeViewMode;
        }

        private void LateUpdate()
        {
            var move = _personAction["Look"].ReadValue<Vector2>();
            _personManager.RotateCamera(move.x, move.y);
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

                if (GameSystem.Instance.NowViewMode.Value == GameSystem.ViewMode.REAPER ||
                    GameSystem.Instance.NowViewMode.Value == GameSystem.ViewMode.TPV)
                {
                    GetComponent<PlayerInput>().SwitchCurrentActionMap("Reaper");
                }
            }
        }
        #endregion
    }
}