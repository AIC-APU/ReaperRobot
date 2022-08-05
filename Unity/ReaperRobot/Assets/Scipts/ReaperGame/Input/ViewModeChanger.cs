using UnityEngine;
using UnityEngine.InputSystem;

namespace smart3tene.Reaper
{
    [RequireComponent(typeof(PlayerInput))]
    public class ViewModeChanger : MonoBehaviour
    {
        #region Private Fields
        private InputActionMap _reaperActionMap;
        private InputActionMap _personActionMap;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            _reaperActionMap = GetComponent<PlayerInput>().actions.FindActionMap("Reaper");
            _personActionMap = GetComponent<PlayerInput>().actions.FindActionMap("Person");

            //closeAppを登録
            _reaperActionMap["ChangeMode"].started += ChangeViewMode;
            _personActionMap["ChangeMode"].started += ChangeViewMode;

            _reaperActionMap["ChangeReaperAndPerson"].started += ChangeReaperAndPerson;
            _personActionMap["ChangeReaperAndPerson"].started += ChangeReaperAndPerson;
        }

        private void OnDestroy()
        {
            _reaperActionMap["ChangeMode"].started -= ChangeViewMode;
            _personActionMap["ChangeMode"].started -= ChangeViewMode;

            _reaperActionMap["ChangeReaperAndPerson"].started -= ChangeReaperAndPerson;
            _personActionMap["ChangeReaperAndPerson"].started -= ChangeReaperAndPerson;
        }
        #endregion

        #region Private method
        private void ChangeViewMode(InputAction.CallbackContext obj)
        {
            ViewMode.ChangeViewMode();
        }

        private void ChangeReaperAndPerson(InputAction.CallbackContext obj)
        {
            ViewMode.ChangeReaperAndPerson();
        }
        #endregion
    }
}

