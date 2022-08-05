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
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            _reaperActionMap = GetComponent<PlayerInput>().actions.FindActionMap("Reaper");
            _personActionMap = GetComponent<PlayerInput>().actions.FindActionMap("Person");

            //closeAppを登録
            _reaperActionMap["Menu"].started += OpenMenu;
            _personActionMap["Menu"].started += OpenMenu;
        }

        private void OnDestroy()
        {
            _reaperActionMap["Menu"].started -= OpenMenu;
            _personActionMap["Menu"].started -= OpenMenu;
        }
        #endregion

        #region Private method
        private void OpenMenu(InputAction.CallbackContext obj)
        {
            ReaperEventManager.InvokeMenuEvent();
        }
        #endregion
    }

}
