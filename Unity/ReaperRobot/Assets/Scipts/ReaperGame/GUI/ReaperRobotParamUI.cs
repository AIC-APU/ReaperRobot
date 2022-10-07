using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace smart3tene.Reaper 
{
    public class ReaperRobotParamUI : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField] private ReaperManager _reaperManager;

        [Header("UIs")]
        [SerializeField] private GameObject _robotParameterPanel;

        [Header("Damping Rate")]
        [SerializeField] private TMP_InputField _dampingInputField;
        [SerializeField] private TMP_Text _dampingRateText;
        [SerializeField] private WheelCollider _wheelColliderL2;
        [SerializeField] private WheelCollider _wheelColliderL3;
        [SerializeField] private WheelCollider _wheelColliderR2;
        [SerializeField] private WheelCollider _wheelColliderR3;

        [Header("Move Torque")]
        [SerializeField] private TMP_InputField _moveTorqueInputField;
        [SerializeField] private TMP_Text _moveTorqueText;

        [Header("Roate Torque")]
        [SerializeField] private TMP_InputField _rotateTorqueInputField;
        [SerializeField] private TMP_Text _rotateTorqueText;

        [Header("Torque Rate at Cutting")]
        [SerializeField] private TMP_InputField _torqueRateInputField;
        [SerializeField] private TMP_Text _torqueRateTorqueText;
        #endregion

        #region Private Fields

        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            //テキストの更新
            _wheelColliderL2
                .ObserveEveryValueChanged(x => x.wheelDampingRate)
                .Subscribe(x => _dampingRateText.text = x.ToString())
                .AddTo(this);

            _reaperManager
                .ObserveEveryValueChanged(x => x.moveTorque)
                .Subscribe(x => _moveTorqueText.text = x.ToString())
                .AddTo(this);

            _reaperManager
                .ObserveEveryValueChanged(x => x.rotateTorque)
                .Subscribe(x => _rotateTorqueText.text = x.ToString())
                .AddTo(this);

            _reaperManager
                .ObserveEveryValueChanged(x => x.torqueRateAtCutting)
                .Subscribe(x => _torqueRateTorqueText.text = x.ToString())
                .AddTo(this);
        }
        #endregion

        #region Public method for button
        public void OnClickDampingSet()
        {
            if (_dampingInputField.text == "") return;

            var value = float.Parse(_dampingInputField.text);

            _wheelColliderL2.wheelDampingRate = value;
            _wheelColliderL3.wheelDampingRate = value;
            _wheelColliderR2.wheelDampingRate = value;
            _wheelColliderR3.wheelDampingRate = value;

            _dampingInputField.text = "";
        }

        public void OnClickMoveTorqueSet()
        {
            if (_moveTorqueInputField.text == "") return;

            var value = float.Parse(_moveTorqueInputField.text);
            _reaperManager.moveTorque = value;

            _moveTorqueInputField.text = "";
        }

        public void OnClickRotateTorqueSet()
        {
            if (_rotateTorqueInputField.text == "") return;

            var value = float.Parse(_rotateTorqueInputField.text);
            _reaperManager.rotateTorque = value;

            _rotateTorqueInputField.text = "";
        }

        public void OnClickTorqueRateAtCuttingSet()
        {
            if (_torqueRateInputField.text == "") return;

            var value = float.Parse(_torqueRateInputField.text);
            value = Mathf.Clamp(value, 0f, 1f);
            _reaperManager.torqueRateAtCutting = value;

            _torqueRateInputField.text = "";
        }
        #endregion
    }

}