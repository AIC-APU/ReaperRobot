using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace smart3tene.Reaper
{
    [RequireComponent(typeof(PlayerInput))]
    public class CSVPlayWithInput : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField] private Button _shadowPlayButton;
        [SerializeField] private Button _robotPlayButton;

        [SerializeField] private Button _shadowPauseButton;
        [SerializeField] private Button _robotPauseButton;
        #endregion

        #region Private Fields
        private InputActionMap _reaperActionMap;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            _reaperActionMap = GetComponent<PlayerInput>().actions.FindActionMap("Reaper");

            //AllPlayの登録
            _reaperActionMap["CSVPlay"].started += AllPlay;
        }
        private void OnDisable()
        {
            //AllPlayの登録削除
            _reaperActionMap["CSVPlay"].started -= AllPlay;
        }
        #endregion

        #region Private method
        private void AllPlay(InputAction.CallbackContext obj)
        {
            //両方のPlayボタンが押せる状態なら押す
            if(_shadowPlayButton.IsActive() && _shadowPlayButton.interactable 
                && _robotPlayButton.IsActive() && _robotPlayButton.interactable)
            {
                _shadowPlayButton.onClick.Invoke();
                _robotPlayButton.onClick.Invoke();
            }
            //両方のPauseボタンが押せる状況なら押す
            else if(_shadowPauseButton.IsActive() && _shadowPauseButton.interactable 
                && _robotPauseButton.IsActive() && _robotPauseButton.interactable)
            {
                _shadowPauseButton.onClick.Invoke();
                _robotPauseButton.onClick.Invoke();
            }
        }
        #endregion
    }

}