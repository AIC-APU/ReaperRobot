using UnityEngine;
using UnityEngine.InputSystem;

namespace ReaperRobot.Scripts.UnityComponent.Menu
{
    [RequireComponent(typeof(PlayerInput))]
    public class MenuOpner : MonoBehaviour
    {
        #region Private Fields
        private PlayerInput _playerInput;
        private InputActionMap _menuMap;
        private bool _isMenuOpen = false;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _menuMap = _playerInput.actions.FindActionMap("Menu");
            _menuMap.Enable();

            _menuMap["Open"].started += MenuAction;
        }

        private void OnDestroy()
        {
            _menuMap["Open"].started -= MenuAction;
            Time.timeScale = 1;
        }
        #endregion

        #region Private method
        private void MenuAction(InputAction.CallbackContext obj)
        {
            if (_isMenuOpen)
            {
            }
            else
            {

            }
        }

        private void Open()
        {
            //メニューを開いたというフラグを立てる
            _isMenuOpen = true;

            //コントローラ操作をできないようにする
            _playerInput.enabled = false;

            //タイマーが動いていたらタイマーを止める


            //ゲームを停止
            Time.timeScale = 0;
        }

        private void Close()
        {
            //メニューを開いたというフラグを立てる
            _isMenuOpen = false;

            //コントローラ操作を可能に
            _playerInput.enabled = true;

            //タイマーを再び起動


            //ゲームを停止
            Time.timeScale = 1;
        }
        #endregion
    }
}
