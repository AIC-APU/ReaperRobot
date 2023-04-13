using UnityEngine;
using UnityEngine.InputSystem;

namespace Plusplus.ReaperRobot.Scripts.UnityComponent.CloseApp
{
    [RequireComponent(typeof(PlayerInput))]
    public class AppCloser : MonoBehaviour
    {
        #region Private Fields
        private InputActionMap _closeActionMap;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            _closeActionMap = GetComponent<PlayerInput>().actions.FindActionMap("CloseApp");
            _closeActionMap.Enable();

            //closeAppを登録
            _closeActionMap["CloseApp"].started += CloseApp;
        }

        private void OnDestroy()
        {
            _closeActionMap["CloseApp"].started -= CloseApp;
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
