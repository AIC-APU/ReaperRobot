using System;
using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.Unitycomponent.TestRobot
{
    public class TestRobotManager : MonoBehaviour
    {
        #region Serialized Private Field
        [Header("Wheel Collider")]
        [SerializeField] private WheelCollider _wheelColliderL1;
        [SerializeField] private WheelCollider _wheelColliderL2;
        [SerializeField] private WheelCollider _wheelColliderR1;
        [SerializeField] private WheelCollider _wheelColliderR2;

        [Header("Crawler")]
        [SerializeField] private Animator _crawlerL;
        [SerializeField] private Animator _crawlerR;

        [Header("Center of Gravity")]
        [SerializeField] private Vector3 _centerOfGravity = new(0, 0, 0);
        #endregion

        #region Public Field
        //トルク関連の値
        [NonSerialized] public float rotateTorque = 100;
        [NonSerialized] public float moveTorque = 100f;
        [NonSerialized] public float torqueRateAtCutting = 0.5f;
        #endregion

        #region Readonly Field
        readonly float brakeTorque = 500f;
        #endregion

        #region MonoBehaviour Callbacks
        void Start()
        {
            //重心の設定
            GetComponent<Rigidbody>().centerOfMass = _centerOfGravity;
        }

        void Update()
        {
            //アニメーション　
            _crawlerL.SetFloat("WheelTorque", _wheelColliderL2.rpm / 70f);
            _crawlerR.SetFloat("WheelTorque", _wheelColliderR2.rpm / 70f);
        }
        #endregion

        #region Public method
        public void Move(float horizontal, float vertical)
        {
            if (horizontal == 0 && vertical == 0 && _wheelColliderL2.motorTorque == 0 && _wheelColliderR2.motorTorque == 0)
            {
                return;
            }

            //入力値の範囲を制限
            horizontal = Mathf.Clamp(horizontal, -1, 1);
            vertical = Mathf.Clamp(vertical, -1, 1);

            //左右車輪のトルクを計算
            var torqueL = moveTorque * vertical;
            var torqueR = moveTorque * vertical;

            torqueL += rotateTorque * horizontal;
            torqueR -= rotateTorque * horizontal;

            //WheelColliderに反映
            _wheelColliderL1.motorTorque = torqueL;
            _wheelColliderL2.motorTorque = torqueL;
            _wheelColliderR1.motorTorque = torqueR;
            _wheelColliderR2.motorTorque = torqueR;
        }

        public void PutOnBrake()
        {
            _wheelColliderL1.brakeTorque = brakeTorque;
            _wheelColliderL2.brakeTorque = brakeTorque;
            _wheelColliderR1.brakeTorque = brakeTorque;
            _wheelColliderR2.brakeTorque = brakeTorque;
        }

        public void ReleaseBrake()
        {
            _wheelColliderL1.brakeTorque = 0;
            _wheelColliderL2.brakeTorque = 0;
            _wheelColliderR1.brakeTorque = 0;
            _wheelColliderR2.brakeTorque = 0;
        }
        #endregion
    }
}