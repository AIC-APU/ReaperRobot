using UnityEngine;
using UnityEngine.InputSystem;

namespace smart3tene.Reaper
{
    [RequireComponent(typeof(PlayerInput))]
    public class AppCloser : MonoBehaviour
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
            _reaperActionMap["CloseApp"].started += CloseApp;
            _personActionMap["CloseApp"].started += CloseApp;
        }

        private void OnDestroy()
        {
            _reaperActionMap["CloseApp"].started -= CloseApp;
            _personActionMap["CloseApp"].started -= CloseApp;
        }
        #endregion

        #region Private method
        private void CloseApp(InputAction.CallbackContext obj)
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        #endregion
    }

}
