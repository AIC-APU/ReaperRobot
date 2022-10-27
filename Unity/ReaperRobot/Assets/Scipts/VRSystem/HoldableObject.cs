using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;

namespace smart3tene
{
    [RequireComponent(typeof(Rigidbody))]
    public class HoldableObject : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField] private bool _useGravityDefault = false;
        #endregion

        #region Private Fields
        private GameObject _leftOVRHandPrefab;
        private GameObject _rightOVRHandPrefab;
        private OVRHand _leftHand;
        private OVRHand _rightHand;
        private OVRSkeleton _leftSkeleton;
        private OVRSkeleton _rightSkeleton;
        private Transform _leftTransform;
        private Transform _rightTransform;

        private Rigidbody _rigidbody;

        private bool _isTouchLeft = false;
        private bool _isTouchRight = false;
        private bool _isHold = false;
        #endregion

        #region MonoBehaviour Callbacks
        void Start()
        {
            _leftOVRHandPrefab  = GameObject.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor/OVRHandPrefab");
            _rightOVRHandPrefab = GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor/OVRHandPrefab");

            _leftHand       = _leftOVRHandPrefab.GetComponent<OVRHand>();
            _rightHand      = _rightOVRHandPrefab.GetComponent<OVRHand>();
            _leftSkeleton   = _leftOVRHandPrefab.GetComponent<OVRSkeleton>();
            _rightSkeleton  = _rightOVRHandPrefab.GetComponent<OVRSkeleton>();
            _leftTransform  = _leftOVRHandPrefab.transform;
            _rightTransform = _rightOVRHandPrefab.transform;

            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = _useGravityDefault;
        }

        private void Update()
        {
            if(_leftHand.IsTracked && _isTouchLeft && _leftHand.GetFingerPinchStrength(OVRHand.HandFinger.Thumb) >= 0.7f)
            {
                //左手でオブジェクトを持つ
                HoldThisObject(_leftHand);
            }
            else if(_rightHand.IsTracked && _isTouchRight && _rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Thumb) >= 0.7f)
            {
                //右手でオブジェクトを持つ
                HoldThisObject(_rightHand);
            }
            else if(_isHold)
            {
                //オブジェクトが離れる
                ReleaseThisObject();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.IsChildOf(_leftTransform))
            {
                _isTouchLeft = true;
            }
            else if(collision.transform.IsChildOf(_rightTransform))
            {
                _isTouchRight = true;
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collision.transform.IsChildOf(_leftTransform))
            {
                _isTouchLeft = true;
            }
            else if (collision.transform.IsChildOf(_rightTransform))
            {
                _isTouchRight = true;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.transform.IsChildOf(_leftTransform))
            {
                _isTouchLeft = false;
            }
            else if (collision.transform.IsChildOf(_rightTransform))
            {
                _isTouchRight = false;
            }
        }
        #endregion

        #region Private method
        private void HoldThisObject(OVRHand hand)
        {
            //オブジェクトが持った時に動かないように設定
            _rigidbody.useGravity = false;
            _rigidbody.freezeRotation = true;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            //オブジェクトの位置をピンチの位置に
            var pos = hand.PointerPose.position;
            transform.position = pos;

            //フラグの書き換え
            _isHold = true;
        }

        private void ReleaseThisObject()
        {
            //オブジェクトが自由に動くように設定
            _rigidbody.useGravity = true;
            _rigidbody.freezeRotation = false;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            //フラグの書き換え
            _isHold = false;
        }
        #endregion
    }
}