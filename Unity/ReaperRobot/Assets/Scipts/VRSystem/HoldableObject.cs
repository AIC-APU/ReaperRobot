using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;
using UniRx;
using UniRx.Triggers;

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

        private Vector3 _defaultPos;
        private Vector3 _defaultAng;
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

            _defaultPos = transform.position;
            _defaultAng = transform.eulerAngles;

            //初めてオブジェクトを触れるまで、オブジェクトは動かない
            if (_useGravityDefault) return;
            this.UpdateAsObservable()
                .TakeUntil(this.UpdateAsObservable().Where(_ => _isHold))
                .Subscribe(_ =>
                {
                    transform.position = _defaultPos;
                    transform.eulerAngles = _defaultAng;
                    _rigidbody.velocity = Vector3.zero;
                    _rigidbody.angularVelocity = Vector3.zero;
                })
                .AddTo(this);
        }

        private void Update()
        {
            if(_leftHand.IsTracked && _isTouchLeft && isGrab(_leftHand, _leftSkeleton))
            {
                //左手でオブジェクトを持つ
                HoldThisObject(_leftOVRHandPrefab);
            }
            else if(_rightHand.IsTracked && _isTouchRight && isGrab(_rightHand, _rightSkeleton))
            {
                //右手でオブジェクトを持つ
                HoldThisObject(_rightOVRHandPrefab);
            }
            else if(_isHold)
            {
                //オブジェクトが離れる
                ReleaseThisObject();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            //個々の条件、手じゃなくて、指にしたい

            if (collision.gameObject.name != "Hand_WristRoot_CapsuleRigidbody") return;

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
            if (collision.gameObject.name != "Hand_WristRoot_CapsuleRigidbody") return;

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
            if (collision.gameObject.name != "Hand_WristRoot_CapsuleRigidbody") return;

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
        private void HoldThisObject(GameObject handPrefab)
        {
            //オブジェクトが持った時に動かないように設定
            _rigidbody.useGravity = false;
            _rigidbody.freezeRotation = true;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            //このオブジェクトを手の子にする
            gameObject.transform.parent = handPrefab.transform;

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

            //親子関係を初期化
            gameObject.transform.parent = null;

            //フラグの書き換え
            _isHold = false;
        }

        private bool isGrab(OVRHand hand, OVRSkeleton skelton)
        {
            //各指の曲がり具合を調べる
            var isThumBend = isBending(hand, skelton, 0.95f, OVRSkeleton.BoneId.Hand_Thumb2, OVRSkeleton.BoneId.Hand_Thumb3, OVRSkeleton.BoneId.Hand_ThumbTip);
            var isIndexBend = isBending(hand, skelton, 0.9f, OVRSkeleton.BoneId.Hand_Index1, OVRSkeleton.BoneId.Hand_Index2, OVRSkeleton.BoneId.Hand_Index3, OVRSkeleton.BoneId.Hand_IndexTip);
            var isMiddleBend = isBending(hand, skelton, 0.9f, OVRSkeleton.BoneId.Hand_Middle1, OVRSkeleton.BoneId.Hand_Middle2, OVRSkeleton.BoneId.Hand_Middle3, OVRSkeleton.BoneId.Hand_MiddleTip);
            var isRingBend = isBending(hand, skelton, 0.9f, OVRSkeleton.BoneId.Hand_Ring1, OVRSkeleton.BoneId.Hand_Ring2, OVRSkeleton.BoneId.Hand_Ring3, OVRSkeleton.BoneId.Hand_RingTip);
            var isPinkyend = isBending(hand, skelton, 0.9f, OVRSkeleton.BoneId.Hand_Pinky0, OVRSkeleton.BoneId.Hand_Pinky1, OVRSkeleton.BoneId.Hand_Pinky2, OVRSkeleton.BoneId.Hand_Pinky3, OVRSkeleton.BoneId.Hand_PinkyTip);

            //親指といずれかの指が曲がっていたらtrue
            return isThumBend && (isIndexBend || isMiddleBend || isRingBend || isPinkyend);

            #region Local Method
            bool isBending(OVRHand hand, OVRSkeleton skelton, float threshold, params OVRSkeleton.BoneId[] boneids)
                {
                    if (!hand.IsTracked) return false;
                    if (boneids.Length < 3) return false; //調べようがない

                    Vector3? oldVec = null;
                    var dot = 1.0f;
                    for (var index = 0; index < boneids.Length - 1; index++)
                    {
                        var v = (skelton.Bones[(int)boneids[index + 1]].Transform.position - skelton.Bones[(int)boneids[index]].Transform.position).normalized;
                        if (oldVec.HasValue)
                        {
                            dot *= Vector3.Dot(v, oldVec.Value); //内積の値を総乗していく
                        }
                        oldVec = v;//ひとつ前の指ベクトル
                    }
                    return dot < threshold;
                }
            #endregion
        }
        #endregion
    }
}