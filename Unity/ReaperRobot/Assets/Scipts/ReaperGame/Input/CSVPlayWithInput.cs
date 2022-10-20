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
        [SerializeField] private Button _recordButton;
        [SerializeField] private Button _shadowPlayButton;
        [SerializeField] private Button _robotPlayButton;

        [SerializeField] private Button _recordStopButton;
        [SerializeField] private Button _shadowStopButton;
        [SerializeField] private Button _robotStopButton;
        #endregion

        #region Private Fields
        private InputActionMap _reaperActionMap;
        private InputActionMap _personActionMap;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            _reaperActionMap = GetComponent<PlayerInput>().actions.FindActionMap("Reaper");
            _personActionMap = GetComponent<PlayerInput>().actions.FindActionMap("Person");

            //AllPlayの登録
            _reaperActionMap["CSVPlay"].started += AllPlay;
            _personActionMap["CSVPlay"].started += AllPlay;

            //AllStopの登録
            _reaperActionMap["CSVStop"].started += AllStop;
            _personActionMap["CSVStop"].started += AllStop;
        }
        private void OnDisable()
        {
            //AllPlayの登録削除
            _reaperActionMap["CSVPlay"].started -= AllPlay;
            _personActionMap["CSVPlay"].started -= AllPlay;

            //AllStopの登録削除
            _reaperActionMap["CSVStop"].started -= AllStop;
            _personActionMap["CSVStop"].started -= AllStop;
        }
        #endregion

        #region Private method
        private void AllPlay(InputAction.CallbackContext obj)
        {
            if (_recordButton.interactable) _recordButton.onClick.Invoke();
            if (_shadowPlayButton.interactable) _shadowPlayButton.onClick.Invoke();
            if (_robotPlayButton.interactable) _robotPlayButton.onClick.Invoke();
        }

        private void AllStop(InputAction.CallbackContext obj)
        {
            if (_recordStopButton.interactable) _recordStopButton.onClick.Invoke();
            if (_shadowStopButton.interactable) _shadowStopButton.onClick.Invoke();
            if (_robotStopButton.interactable) _robotStopButton.onClick.Invoke();
        }
        #endregion
    }

}