using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ReaperRobot.Scripts.UnityComponent.ReaperRobot.GUI
{
    public class ReaperRobotParamUI : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField] private ReaperManager _reaperManager;

        [Header("Wheels")]
        [SerializeField] private List<WheelCollider> _wheelColliders = new List<WheelCollider>();

        [Header("UIs")]
        [SerializeField] private GameObject _robotParameterPanel;

        [Header("Damping Rate")]
        [SerializeField] private TMP_InputField _dampingInputField;

        [Header("Move Torque")]
        [SerializeField] private TMP_InputField _moveTorqueInputField;

        [Header("Torque Rate at Cutting")]
        [SerializeField] private TMP_InputField _torqueRateInputField;

        [Header("Mass")]
        [SerializeField] private TMP_InputField _robotMassInputField;
        [SerializeField] private Rigidbody _robotBody;

        [Header("Friction")]
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
            _dampingInputField.text = _wheelColliders[0].wheelDampingRate.ToString();
            _moveTorqueInputField.text = _reaperManager.moveTorque.ToString();
            _torqueRateInputField.text = _reaperManager.torqueRateAtCutting.ToString();
            _robotMassInputField.text = _robotBody.mass.ToString();
            _forwardFrictionField.text = _wheelColliders[0].forwardFriction.stiffness.ToString();
            _sidewaysFrictionField.text = _wheelColliders[0].sidewaysFriction.stiffness.ToString();

            //デフォルト値の設定
            _defaultDampingRate = _wheelColliders[0].wheelDampingRate;
            _defaultMoveTorque = _reaperManager.moveTorque;
            _defaultTorqueRate = _reaperManager.torqueRateAtCutting;
            _defaultMass = _robotBody.mass;
            _defaultForwardFriction = _wheelColliders[0].forwardFriction.stiffness;
            _defaultSidewaysFriction = _wheelColliders[0].sidewaysFriction.stiffness;
        }
        #endregion

        #region Public method for InputFields

        public void OnEndEditDanpingRate()
        {
            if (_dampingInputField.text == "" || _dampingInputField.text == "-")
            {
                _dampingInputField.text = _wheelColliders[0].wheelDampingRate.ToString();
                return;
            }

            var value = Mathf.Abs(float.Parse(_dampingInputField.text));

            foreach (WheelCollider wheel in _wheelColliders)
            {
                wheel.wheelDampingRate = value;
            }

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
        public void OnEndEditTorqueRate()
        {
            if (_torqueRateInputField.text == "" || _torqueRateInputField.text == "-")
            {
                _torqueRateInputField.text = _reaperManager.torqueRateAtCutting.ToString();
                return;
            }

            var value = Mathf.Abs(float.Parse(_torqueRateInputField.text));
            value = Mathf.Clamp(value, 0, 1);

            _reaperManager.torqueRateAtCutting = value;
            _torqueRateInputField.text = value.ToString();
        }
        public void OnEndEditRobotMass()
        {
            if (_robotMassInputField.text == "" || _robotMassInputField.text == "-")
            {
                _robotMassInputField.text = _robotBody.mass.ToString();
                return;
            }

            var value = Mathf.Abs(float.Parse(_robotMassInputField.text));

            _robotBody.mass = value;
            _robotMassInputField.text = value.ToString();
        }
        public void OnEndEditForwardFriction()
        {
            if (_forwardFrictionField.text == "" || _forwardFrictionField.text == "-")
            {
                _forwardFrictionField.text = _wheelColliders[0].forwardFriction.stiffness.ToString();
                return;
            }

            var value = Mathf.Abs(float.Parse(_forwardFrictionField.text));
            value = Mathf.Clamp(value, 0, 1);

            foreach (WheelCollider wheel in _wheelColliders)
            {
                var ForwardFriction = wheel.forwardFriction;
                ForwardFriction.stiffness = value;
                wheel.forwardFriction = ForwardFriction;
            }
            _forwardFrictionField.text = value.ToString();
        }
        public void OnEndEditSidewaysFriction()
        {
            if (_sidewaysFrictionField.text == "" || _sidewaysFrictionField.text == "-")
            {
                _sidewaysFrictionField.text = _wheelColliders[0].sidewaysFriction.stiffness.ToString();
                return;
            }

            var value = Mathf.Abs(float.Parse(_sidewaysFrictionField.text));
            value = Mathf.Clamp(value, 0, 1);

            foreach (WheelCollider wheel in _wheelColliders)
            {
                var sidewaysFriction = wheel.sidewaysFriction;
                sidewaysFriction.stiffness = value;
                wheel.sidewaysFriction = sidewaysFriction;
            }
            _sidewaysFrictionField.text = value.ToString();
        }
        public void OnClickReset()
        {
            //Damping Rate
            foreach (WheelCollider wheel in _wheelColliders)
            {
                wheel.wheelDampingRate = _defaultDampingRate;
            }
            _dampingInputField.text = _defaultDampingRate.ToString();

            //Move Torque
            _reaperManager.moveTorque = _defaultMoveTorque;
            _moveTorqueInputField.text = _defaultMoveTorque.ToString();

            //TorqueRateAtCutting
            _reaperManager.torqueRateAtCutting = _defaultTorqueRate;
            _torqueRateInputField.text = _defaultTorqueRate.ToString();

            //Mass
            _robotBody.mass = _defaultMass;
            _robotMassInputField.text = _defaultMass.ToString();

            //Friction Stiffness
            foreach (WheelCollider wheel in _wheelColliders)
            {
                var ForwardFriction = wheel.forwardFriction;
                ForwardFriction.stiffness = _defaultForwardFriction;
                wheel.forwardFriction = ForwardFriction;

                var sidewaysFriction = wheel.sidewaysFriction;
                sidewaysFriction.stiffness = _defaultSidewaysFriction;
                wheel.sidewaysFriction = sidewaysFriction;
            }
            _forwardFrictionField.text = _defaultForwardFriction.ToString();
            _sidewaysFrictionField.text = _defaultSidewaysFriction.ToString();
        }
        #endregion
    }

}