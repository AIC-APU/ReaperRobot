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

        [Header("Torque Rate at Cutting")]
        [SerializeField] private TMP_InputField _torqueRateInputField;

        [Header("Mass")]
        [SerializeField] private TMP_InputField _robotMassInputField;
        [SerializeField] private Rigidbody _robotBody;
        #endregion

        #region Private Fields
        private float _defaultDampingRate;
        private float _defaultMoveTorque;
        private float _defaultTorqueRate;
        private float _defaultMass;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            //テキストの更新
            _dampingInputField.text = _wheelColliderL2.wheelDampingRate.ToString();
            _moveTorqueInputField.text = _reaperManager.moveTorque.ToString();
            _torqueRateInputField.text = _reaperManager.torqueRateAtCutting.ToString();
            _robotMassInputField.text = _robotBody.mass.ToString();

            //デフォルト値の設定
            _defaultDampingRate = _wheelColliderL2.wheelDampingRate;
            _defaultMoveTorque = _reaperManager.moveTorque;
            _defaultTorqueRate = _reaperManager.torqueRateAtCutting;
            _defaultMass = _robotBody.mass;
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
        public void OnClickReset()
        {
            _wheelColliderL2.wheelDampingRate = _defaultDampingRate;
            _wheelColliderL3.wheelDampingRate = _defaultDampingRate;
            _wheelColliderR2.wheelDampingRate = _defaultDampingRate;
            _wheelColliderR3.wheelDampingRate = _defaultDampingRate;
            _dampingInputField.text = _defaultDampingRate.ToString();

            _reaperManager.moveTorque = _defaultMoveTorque;
            _moveTorqueInputField.text = _defaultMoveTorque.ToString();

            _reaperManager.torqueRateAtCutting = _defaultTorqueRate;
            _torqueRateInputField.text = _defaultTorqueRate.ToString();

            _robotBody.mass = _defaultMass;
            _robotMassInputField.text = _defaultMass.ToString();
        }
        #endregion
    }

}