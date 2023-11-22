using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.View.ReaperRobot
{
    public class ReaperRobotParamUI : MonoBehaviour
    {
        #region Serialized Private Fields
        [Header("Parameters")]
        [SerializeField] private ReaperParameter _targetParameter;

        [Header("UIs")]
        [SerializeField] private GameObject _robotParameterPanel;
        [SerializeField] private TMP_InputField _dampingInputField;
        [SerializeField] private TMP_InputField _moveTorqueInputField;
        [SerializeField] private TMP_InputField _torqueRateInputField;
        [SerializeField] private TMP_InputField _robotMassInputField;
        [SerializeField] private TMP_InputField _forwardFrictionField;
        [SerializeField] private TMP_InputField _sidewaysFrictionField;
        #endregion

        #region Private Fields
        private float _defaultDampingRate;
        private float _defaultMoveTorque;
        private float _defaultTorqueRate;
        private float _defaultMass;
        private float _defaultForwardFriction;
        private float _defaultSidewaysFriction;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            //テキストの更新
            _dampingInputField.text = _targetParameter.DampingRate.Value.ToString();
            _moveTorqueInputField.text = _targetParameter.MoveTorque.Value.ToString();
            _torqueRateInputField.text = _targetParameter.TorqueRateAtCutting.Value.ToString();
            _robotMassInputField.text = _targetParameter.RobotMath.Value.ToString();
            _forwardFrictionField.text = _targetParameter.ForwardFriction.Value.ToString();
            _sidewaysFrictionField.text = _targetParameter.SidewaysFriction.Value.ToString();

            //デフォルト値の設定
            _defaultDampingRate = _targetParameter.DampingRate.Value;
            _defaultMoveTorque = _targetParameter.MoveTorque.Value;
            _defaultTorqueRate = _targetParameter.TorqueRateAtCutting.Value;
            _defaultMass = _targetParameter.RobotMath.Value;
            _defaultForwardFriction = _targetParameter.ForwardFriction.Value;
            _defaultSidewaysFriction = _targetParameter.SidewaysFriction.Value;
        }

        void OnDestroy()
        {
            //デフォルト値に戻す処理
            //一応書いておくが、必要なければ消していい
            OnClickReset();
        }
        #endregion

        #region Public method for InputFields
        public void OnEndEditDanpingRate()
        {
            if (_dampingInputField.text == "" || _dampingInputField.text == "-")
            {
                _dampingInputField.text = _targetParameter.DampingRate.Value.ToString();
                return;
            }

            var value = Mathf.Abs(float.Parse(_dampingInputField.text));
            _targetParameter.DampingRate.Value = value;
            _dampingInputField.text = value.ToString();
        }

        public void OnEndEditMoveTorque()
        {
            if (_moveTorqueInputField.text == "" || _moveTorqueInputField.text == "-")
            {
                _moveTorqueInputField.text = _targetParameter.MoveTorque.Value.ToString();
                return;
            }

            var value = Mathf.Abs(float.Parse(_moveTorqueInputField.text));
            _targetParameter.MoveTorque.Value = value;
            _moveTorqueInputField.text = value.ToString();

        }
        public void OnEndEditTorqueRate()
        {
            if (_torqueRateInputField.text == "" || _torqueRateInputField.text == "-")
            {
                _torqueRateInputField.text = _targetParameter.TorqueRateAtCutting.Value.ToString();
                return;
            }

            var value = Mathf.Abs(float.Parse(_torqueRateInputField.text));
            value = Mathf.Clamp(value, 0, 1);

            _targetParameter.TorqueRateAtCutting.Value = value;
            _torqueRateInputField.text = value.ToString();
        }
        public void OnEndEditRobotMass()
        {
            if (_robotMassInputField.text == "" || _robotMassInputField.text == "-")
            {
                _robotMassInputField.text = _targetParameter.RobotMath.Value.ToString();
                return;
            }

            var value = Mathf.Abs(float.Parse(_robotMassInputField.text));

            _targetParameter.RobotMath.Value = value;
            _robotMassInputField.text = value.ToString();
        }
        public void OnEndEditForwardFriction()
        {
            if (_forwardFrictionField.text == "" || _forwardFrictionField.text == "-")
            {
                _forwardFrictionField.text = _targetParameter.ForwardFriction.Value.ToString();
                return;
            }

            var value = Mathf.Abs(float.Parse(_forwardFrictionField.text));
            value = Mathf.Clamp(value, 0, 1);
            _targetParameter.ForwardFriction.Value = value;
            _forwardFrictionField.text = value.ToString();
        }
        public void OnEndEditSidewaysFriction()
        {
            if (_sidewaysFrictionField.text == "" || _sidewaysFrictionField.text == "-")
            {
                _sidewaysFrictionField.text = _targetParameter.SidewaysFriction.Value.ToString();
                return;
            }

            var value = Mathf.Abs(float.Parse(_sidewaysFrictionField.text));
            value = Mathf.Clamp(value, 0, 1);
            _targetParameter.SidewaysFriction.Value = value;
            _sidewaysFrictionField.text = value.ToString();
        }
        public void OnClickReset()
        {
            _targetParameter.DampingRate.Value = _defaultDampingRate;
            _targetParameter.MoveTorque.Value = _defaultMoveTorque;
            _targetParameter.TorqueRateAtCutting.Value = _defaultTorqueRate;
            _targetParameter.RobotMath.Value = _defaultMass;
            _targetParameter.ForwardFriction.Value = _defaultForwardFriction;
            _targetParameter.SidewaysFriction.Value = _defaultSidewaysFriction;
        }
        #endregion
    }

}