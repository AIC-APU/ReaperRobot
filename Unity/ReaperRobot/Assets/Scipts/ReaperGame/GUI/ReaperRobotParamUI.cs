using TMPro;
using UnityEngine;

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
        [SerializeField] private WheelCollider _wheelColliderL2;
        [SerializeField] private WheelCollider _wheelColliderL3;
        [SerializeField] private WheelCollider _wheelColliderR2;
        [SerializeField] private WheelCollider _wheelColliderR3;

        [Header("Move Torque")]
        [SerializeField] private TMP_InputField _moveTorqueInputField;

        [Header("Roate Torque")]
        [SerializeField] private TMP_InputField _rotateTorqueInputField;

        [Header("Torque Rate at Cutting")]
        [SerializeField] private TMP_InputField _torqueRateInputField;
        #endregion

        #region Private Fields
        private float _defaultDampingRate;
        private float _defaultMoveTorque;
        private float _defaultRotateTorque;
        private float _defaultTorqueRate;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            //テキストの更新
            _dampingInputField.text = _wheelColliderL2.wheelDampingRate.ToString();
            _moveTorqueInputField.text = _reaperManager.moveTorque.ToString();
            _rotateTorqueInputField.text = _reaperManager.rotateTorque.ToString();
            _torqueRateInputField.text = _reaperManager.torqueRateAtCutting.ToString();

            //デフォルト値の設定
            _defaultDampingRate = _wheelColliderL2.wheelDampingRate;
            _defaultMoveTorque = _reaperManager.moveTorque;
            _defaultRotateTorque = _reaperManager.rotateTorque;
            _defaultTorqueRate = _reaperManager.torqueRateAtCutting;
        }
        #endregion

        #region Public method for InputFields

        public void OnEndEditDanpingRate()
        {
            if (_dampingInputField.text == "" || _dampingInputField.text == "-")
            {
                _dampingInputField.text = _wheelColliderL2.wheelDampingRate.ToString();
                return;
            }

            var value = Mathf.Abs(float.Parse(_dampingInputField.text));

            _wheelColliderL2.wheelDampingRate = value;
            _wheelColliderL3.wheelDampingRate = value;
            _wheelColliderR2.wheelDampingRate = value;
            _wheelColliderR3.wheelDampingRate = value;

            _dampingInputField.text = value.ToString();
        }

        public void OnEndEditMoveTorque()
        {
            if (_moveTorqueInputField.text == "" || _moveTorqueInputField.text == "-")
            {
                _moveTorqueInputField.text = _reaperManager.moveTorque.ToString();
                return;
            }

            var value = Mathf.Abs(float.Parse(_moveTorqueInputField.text));
            _reaperManager.moveTorque = value;
            _moveTorqueInputField.text = value.ToString();

        }
        public void OnEndEditRotateTorque()
        {
            if (_rotateTorqueInputField.text == "" || _rotateTorqueInputField.text == "-")
            {
                _rotateTorqueInputField.text = _reaperManager.rotateTorque.ToString();
                return;
            }

            var value = Mathf.Abs(float.Parse(_rotateTorqueInputField.text));
            _reaperManager.rotateTorque = value;
            _rotateTorqueInputField.text = value.ToString();
        }
        public void OnEndEditTorqueRate()
        {
            if(_torqueRateInputField.text == "" || _torqueRateInputField.text == "-")
            {
                _torqueRateInputField.text = _reaperManager.torqueRateAtCutting.ToString();
                return;
            }

            var value = Mathf.Abs(float.Parse(_torqueRateInputField.text));
            value = Mathf.Clamp(value, 0, 1);

            _reaperManager.torqueRateAtCutting = value;
            _torqueRateInputField.text = value.ToString();
        }

        public void OnClickReset()
        {
            _wheelColliderL2.wheelDampingRate = _defaultDampingRate;
            _wheelColliderL3.wheelDampingRate = _defaultDampingRate;
            _wheelColliderR2.wheelDampingRate = _defaultDampingRate;
            _wheelColliderR3.wheelDampingRate = _defaultDampingRate;
            _dampingInputField.text = _defaultDampingRate.ToString();

            _reaperManager.moveTorque = _defaultMoveTorque;
            _moveTorqueInputField.text = _defaultMoveTorque.ToString();

            _reaperManager.rotateTorque = _defaultRotateTorque;
            _rotateTorqueInputField.text = _defaultRotateTorque.ToString();

            _reaperManager.torqueRateAtCutting = _defaultTorqueRate;
            _torqueRateInputField.text = _defaultTorqueRate.ToString();
        }
        #endregion
    }

}