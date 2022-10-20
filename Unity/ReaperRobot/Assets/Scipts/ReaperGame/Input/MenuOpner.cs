using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

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
            //メニューを開いたというフラグを立てる
            _isMenuOpen = true;

            //コントローラ操作をできないようにする
            _playerInput.enabled = false;

            //マルチモードなら以下の操作はしない
            if (PhotonNetwork.IsConnected　&& !PhotonNetwork.OfflineMode) return;

            //タイマーが動いていたらタイマーを止める
            _wasUseTimer = GameTimer.IsTimerRunning;
            if (GameTimer.IsTimerRunning) GameTimer.Stop();

            //ゲームを停止
            Time.timeScale = 0;
        }

        private void OnCloseMenuEvent()
        {
            //メニューを開いたというフラグを立てる
            _isMenuOpen = false;

            //コントローラ操作を可能に
            _playerInput.enabled = true;

            //マルチモードなら以下の操作はしない
            if (PhotonNetwork.IsConnected && !PhotonNetwork.OfflineMode) return;

            //タイマーを再び起動
            if (_wasUseTimer) GameTimer.Start();

            //ゲームを停止
            Time.timeScale = 1;
        }
        #endregion
    }

}
