using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.View.OculusIntegration
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

        private ReactiveProperty<bool> _isHoldRight = new(false);
        private ReactiveProperty<bool> _isHoldLeft = new(false);

        private Vector3 _defaultPos;
        private Vector3 _defaultAng;
        private Vector3 _localPos = Vector3.zero;
        private Vector3 _localAng = Vector3.zero;
        #endregion

        #region Readonly Fields
        readonly float _thumbGrabThreshold = 0.95f;
        readonly float _grabThreshold = 0.4f;
        #endregion

        #region MonoBehaviour Callbacks
        void Start()
        {
            //色々値を取得
            _leftOVRHandPrefab  = GameObject.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor/OVRHandPrefab");
            _rightOVRHandPrefab = GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor/OVRHandPrefab");

            _leftHand       = _leftOVRHandPrefab.GetComponent<OVRHand>();
            _rightHand      = _rightOVRHandPrefab.GetComponent<OVRHand>();
            _leftSkeleton   = _leftOVRHandPrefab.GetComponent<OVRSkeleton>();
            _rightSkeleton  = _rightOVRHandPrefab.GetComponent<OVRSkeleton>();
            _leftTransform  = _leftOVRHandPrefab.transform;     //ちなみにこのtransformのPositionは常に原点。位置取得ではなく親子関係の判定のみに使用。
            _rightTransform = _rightOVRHandPrefab.transform;    //これもPositionは常に原点

            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = _useGravityDefault;
            if (!_useGravityDefault) _rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;

            _defaultPos = transform.position;
            _defaultAng = transform.eulerAngles;

            //初めてこのオブジェクトを触れるまで、このオブジェクトは動かない
            if (!_useGravityDefault)
            {
                this.UpdateAsObservable()
                    .TakeUntil(this.UpdateAsObservable().Where(_ => _isHoldRight.Value || _isHoldLeft.Value))
                    .Subscribe(_ =>
                    {
                        transform.position = _defaultPos;
                        transform.eulerAngles = _defaultAng;
                        _rigidbody.velocity = Vector3.zero;
                        _rigidbody.angularVelocity = Vector3.zero;
                        _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
                    })
                    .AddTo(this);
            }

            //各ハンドのトラッキングが外れた場合、タッチフラグをfalseにする
            _leftHand
                .ObserveEveryValueChanged(x => x.IsTracked)
                .Where(x => x == false)
                .Subscribe(_ => _isTouchLeft = false)
                .AddTo(this);

            _rightHand
                .ObserveEveryValueChanged(x => x.IsTracked)
                .Where(x => x == false)
                .Subscribe(_ => _isTouchRight = false)
                .AddTo(this);

            //isHoldフラグがtrueなら物を掴む、falseなら離す
            _isHoldLeft
                .Subscribe(x =>
                {
                    if (x)
                    {
                        HoldThisObject(_leftOVRHandPrefab);
                    }
                    else
                    {
                        ReleaseThisObject();
                    }
                })
                .AddTo(this);

            _isHoldRight
                .Subscribe(x =>
                {
                    if (x)
                    {
                        HoldThisObject(_rightOVRHandPrefab);
                    }
                    else
                    {
                        ReleaseThisObject();
                    }
                })
                .AddTo(this);
        }

        private void Update()
        {
            //このオブジェクトに触っている状態でGrabされた時、このオブジェクトは掴まれる
            _isHoldLeft.Value = _isTouchLeft  && HandTrackingUtility.IsGrab(_leftHand, _leftSkeleton, _thumbGrabThreshold, _grabThreshold) ;
            _isHoldRight.Value = _isTouchRight && HandTrackingUtility.IsGrab(_rightHand, _rightSkeleton, _thumbGrabThreshold, _grabThreshold) ;

            //物を掴んでいる状態の時、ローカル座標の反映
            if (_isHoldLeft.Value || _isHoldRight.Value)
            {
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
                gameObject.transform.localPosition = _localPos;
                gameObject.transform.localEulerAngles = _localAng;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.IsChildOf(_leftTransform) 
                && !HandTrackingUtility.IsGrab(_leftHand, _leftSkeleton, _thumbGrabThreshold, _grabThreshold) 
                && HandTrackingUtility.IsObjectInPalm(_leftHand, _leftSkeleton, gameObject)
                && !_isHoldRight.Value)
            {
                _isTouchLeft = true;
            }
            else if(other.transform.IsChildOf(_rightTransform) 
                && !HandTrackingUtility.IsGrab(_rightHand, _rightSkeleton, _thumbGrabThreshold, _grabThreshold) 
                && HandTrackingUtility.IsObjectInPalm(_rightHand, _rightSkeleton, gameObject)
                && !_isHoldLeft.Value)
            {
                _isTouchRight = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform.IsChildOf(_leftTransform))
            {
                _isTouchLeft = false;
            }
            else if(other.transform.IsChildOf(_rightTransform))
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

            //このオブジェクトの親に、手を設定する
            gameObject.transform.parent = handPrefab.transform;

            //ローカル位置・ローカル回転を取得
            _localPos = gameObject.transform.localPosition;
            _localAng = gameObject.transform.localEulerAngles;
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

            //ローカル位置・ローカル回転を初期化
            _localPos = Vector3.zero;
            _localAng = Vector3.zero;
        }
        #endregion
    }
}