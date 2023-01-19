using UniRx;
using UnityEngine;

namespace smart3tene
{
    public class HandTrackingDebug : MonoBehaviour
    {
        #region Serialized Private Fields
        [Header("Left Hand")]
        [SerializeField] private GameObject _leftHandPrefab;

        [Header("Right Hand")]
        [SerializeField] private GameObject _rightHandPrefab;

        [Header("Display")]
        [SerializeField] private GameObject _leftThumbDis;
        [SerializeField] private GameObject _leftIndexDis;
        [SerializeField] private GameObject _leftMiddleDis;
        [SerializeField] private GameObject _leftRingDis;
        [SerializeField] private GameObject _leftPinkyDis;
        [SerializeField] private GameObject _rightThumbDis;
        [SerializeField] private GameObject _rightIndexDis;
        [SerializeField] private GameObject _rightMiddleDis;
        [SerializeField] private GameObject _rightRingDis;
        [SerializeField] private GameObject _rightPinkyDis;

        [SerializeField] private GameObject _rightInPalmDis;

        [Header("Bend Threshold")]
        [SerializeField] private float _thumbThreshold = 0.95f;
        [SerializeField] private float _threshold = 0.5f;
        #endregion

        #region Private Fields
        private OVRHand _leftHand;
        private OVRSkeleton _leftskeleton;
        private OVRHand _rightHand;
        private OVRSkeleton _rightskeleton;

        private ReactiveProperty<bool> _isLeftThumBend = new(false);
        private ReactiveProperty<bool> _isLeftIndexBend = new(false);
        private ReactiveProperty<bool> _isLeftMiddleBend = new(false);
        private ReactiveProperty<bool> _isLeftRingBend = new(false);
        private ReactiveProperty<bool> _isLeftPinkyBend = new(false);
        private ReactiveProperty<bool> _isRightThumBend = new(false);
        private ReactiveProperty<bool> _isRightIndexBend = new(false);
        private ReactiveProperty<bool> _isRightMiddleBend = new(false);
        private ReactiveProperty<bool> _isRightRingBend = new(false);
        private ReactiveProperty<bool> _isRightPinkyBend = new(false);
        private ReactiveProperty<bool> _isRightInPalm = new(false);
        #endregion

        #region MonoBehaviour Callbacks
        void Start()
        {
            _leftHand      = _leftHandPrefab.GetComponent<OVRHand>();
            _leftskeleton  = _leftHandPrefab.GetComponent<OVRSkeleton>();
            _rightHand     = _rightHandPrefab.GetComponent<OVRHand>();
            _rightskeleton = _rightHandPrefab.GetComponent<OVRSkeleton>();

            _leftHand
                .ObserveEveryValueChanged(x => x.IsTracked)
                .Subscribe(x =>
                {
                    _leftThumbDis.GetComponent<Renderer>().material.color = Color.black;
                    _leftIndexDis.GetComponent<Renderer>().material.color = Color.black;
                    _leftMiddleDis.GetComponent<Renderer>().material.color = Color.black;
                    _leftRingDis.GetComponent<Renderer>().material.color = Color.black;
                    _leftPinkyDis.GetComponent<Renderer>().material.color = Color.black;
                })
                .AddTo(this);

            _rightHand
                .ObserveEveryValueChanged(x => x.IsTracked)
                .Subscribe(x =>
                {
                    _rightThumbDis.GetComponent<Renderer>().material.color = Color.black;
                    _rightIndexDis.GetComponent<Renderer>().material.color = Color.black;
                    _rightMiddleDis.GetComponent<Renderer>().material.color = Color.black;
                    _rightRingDis.GetComponent<Renderer>().material.color = Color.black;
                    _rightPinkyDis.GetComponent<Renderer>().material.color = Color.black;
                })
                .AddTo(this);

            _isLeftThumBend
                .Where(_ => _leftHand.IsTracked)
                .Subscribe(x =>_leftThumbDis.GetComponent<Renderer>().material.color = DisplayColor(x))
                .AddTo(this);

            _isLeftIndexBend
                .Where(_ => _leftHand.IsTracked)
                .Subscribe(x =>_leftIndexDis.GetComponent<Renderer>().material.color = DisplayColor(x))
                .AddTo(this);

            _isLeftMiddleBend
                .Where(_ => _leftHand.IsTracked)
                .Subscribe(x => _leftMiddleDis.GetComponent<Renderer>().material.color = DisplayColor(x))
                .AddTo(this);

            _isLeftRingBend
                .Where(_ => _leftHand.IsTracked)
                .Subscribe(x => _leftRingDis.GetComponent<Renderer>().material.color = DisplayColor(x))
                .AddTo(this);

            _isLeftPinkyBend
                .Where(_ => _leftHand.IsTracked)
                .Subscribe(x => _leftPinkyDis.GetComponent<Renderer>().material.color = DisplayColor(x))
                .AddTo(this);

            _isRightThumBend
                .Where(_ => _rightHand.IsTracked)
                .Subscribe(x => _rightThumbDis.GetComponent<Renderer>().material.color = DisplayColor(x))
                .AddTo(this);

            _isRightIndexBend
                .Where(_ => _rightHand.IsTracked)
                .Subscribe(x => _rightIndexDis.GetComponent<Renderer>().material.color = DisplayColor(x))
                .AddTo(this);

            _isRightMiddleBend
                .Where(_ => _rightHand.IsTracked)
                .Subscribe(x => _rightMiddleDis.GetComponent<Renderer>().material.color = DisplayColor(x))
                .AddTo(this);

            _isRightRingBend
                .Where(_ => _rightHand.IsTracked)
                .Subscribe(x => _rightRingDis.GetComponent<Renderer>().material.color = DisplayColor(x))
                .AddTo(this);

            _isRightPinkyBend
                .Where(_ => _rightHand.IsTracked)
                .Subscribe(x => _rightPinkyDis.GetComponent<Renderer>().material.color = DisplayColor(x))
                .AddTo(this);

            _isRightInPalm
                .Where(_ => _rightHand.IsTracked)
                .Subscribe(x => _rightInPalmDis.GetComponent<Renderer>().material.color = DisplayColor(x))
                .AddTo(this);
        }

        void Update()
        {
            if (_leftHand.IsTracked)
            {
                _isLeftThumBend.Value = HandTrackingUtility.IsBending(_leftHand, _leftskeleton, _thumbThreshold, OVRSkeleton.BoneId.Hand_Thumb2, OVRSkeleton.BoneId.Hand_Thumb3, OVRSkeleton.BoneId.Hand_ThumbTip);
                _isLeftIndexBend.Value = HandTrackingUtility.IsBending(_leftHand, _leftskeleton, _threshold, OVRSkeleton.BoneId.Hand_Index1, OVRSkeleton.BoneId.Hand_Index2, OVRSkeleton.BoneId.Hand_Index3, OVRSkeleton.BoneId.Hand_IndexTip);
                _isLeftMiddleBend.Value = HandTrackingUtility.IsBending(_leftHand, _leftskeleton, _threshold, OVRSkeleton.BoneId.Hand_Middle1, OVRSkeleton.BoneId.Hand_Middle2, OVRSkeleton.BoneId.Hand_Middle3, OVRSkeleton.BoneId.Hand_MiddleTip);
                _isLeftRingBend.Value = HandTrackingUtility.IsBending(_leftHand, _leftskeleton, _threshold, OVRSkeleton.BoneId.Hand_Ring1, OVRSkeleton.BoneId.Hand_Ring2, OVRSkeleton.BoneId.Hand_Ring3, OVRSkeleton.BoneId.Hand_RingTip);
                _isLeftPinkyBend.Value = HandTrackingUtility.IsBending(_leftHand, _leftskeleton, _threshold, OVRSkeleton.BoneId.Hand_Pinky0, OVRSkeleton.BoneId.Hand_Pinky1, OVRSkeleton.BoneId.Hand_Pinky2, OVRSkeleton.BoneId.Hand_Pinky3, OVRSkeleton.BoneId.Hand_PinkyTip);
            }

            if (_rightHand.IsTracked)
            {
                _isRightThumBend.Value = HandTrackingUtility.IsBending(_rightHand, _rightskeleton, _thumbThreshold, OVRSkeleton.BoneId.Hand_Thumb2, OVRSkeleton.BoneId.Hand_Thumb3, OVRSkeleton.BoneId.Hand_ThumbTip);
                _isRightIndexBend.Value = HandTrackingUtility.IsBending(_rightHand, _rightskeleton, _threshold, OVRSkeleton.BoneId.Hand_Index1, OVRSkeleton.BoneId.Hand_Index2, OVRSkeleton.BoneId.Hand_Index3, OVRSkeleton.BoneId.Hand_IndexTip);
                _isRightMiddleBend.Value = HandTrackingUtility.IsBending(_rightHand, _rightskeleton, _threshold, OVRSkeleton.BoneId.Hand_Middle1, OVRSkeleton.BoneId.Hand_Middle2, OVRSkeleton.BoneId.Hand_Middle3, OVRSkeleton.BoneId.Hand_MiddleTip);
                _isRightRingBend.Value = HandTrackingUtility.IsBending(_rightHand, _rightskeleton, _threshold, OVRSkeleton.BoneId.Hand_Ring1, OVRSkeleton.BoneId.Hand_Ring2, OVRSkeleton.BoneId.Hand_Ring3, OVRSkeleton.BoneId.Hand_RingTip);
                _isRightPinkyBend.Value = HandTrackingUtility.IsBending(_rightHand, _rightskeleton, _threshold, OVRSkeleton.BoneId.Hand_Pinky0, OVRSkeleton.BoneId.Hand_Pinky1, OVRSkeleton.BoneId.Hand_Pinky2, OVRSkeleton.BoneId.Hand_Pinky3, OVRSkeleton.BoneId.Hand_PinkyTip);

                _isRightInPalm.Value = HandTrackingUtility.IsObjectInPalm(_rightHand, _rightskeleton, _rightInPalmDis);
            }
        }
        #endregion

        #region Public method

        #endregion

        #region Private method
        private Color DisplayColor(bool isBend)
        {
            if (isBend) return Color.red;
            else return Color.white;
        }
        #endregion
    }

}