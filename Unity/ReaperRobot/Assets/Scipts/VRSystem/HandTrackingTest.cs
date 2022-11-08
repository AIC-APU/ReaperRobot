using Oculus.Interaction.PoseDetection.Debug;
using smart3tene.Reaper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace smart3tene
{

    public class HandTrackingTest : MonoBehaviour
    {
        #region Serialized Private Fields
        //インスペクタビューから現在の値を確認するためのシリアライズ
        [Header("Hand")]
        [SerializeField] private OVRHand _hand; //これはインスペクタビューから設定してほしい
        [SerializeField] private OVRSkeleton _skeleton;

        [Header("Hand Data")]
        [SerializeField] private bool isTracked;
        [SerializeField] private OVRHand.TrackingConfidence confidence = OVRHand.TrackingConfidence.Low;

        [Header("pinch")]
        [SerializeField] private bool isThumbPinch = false;
        [SerializeField] private float ThumbPinchStrength = 0f;
        [SerializeField] private bool isIndexPinch = false;
        [SerializeField] private float IndexPinchStrength = 0f;
        [SerializeField] private bool isMiddlePinch = false;
        [SerializeField] private float MiddlePinchStrength = 0f;
        [SerializeField] private bool isRingPinch = false;
        [SerializeField] private float RingPinchStrength = 0f;
        [SerializeField] private bool isPinkyPinch = false;
        [SerializeField] private float PinkyPinchStrength = 0f;

        [Header("Bend")]
        [SerializeField] private bool isThumbBend = false;
        [SerializeField] private bool isIndexBend = false;
        [SerializeField] private bool isMiddleBend = false;
        [SerializeField] private bool isRingBend = false;
        [SerializeField] private bool isPinkyBend = false;


        [Header("Pointer Pose")]
        [SerializeField] private Vector3 handPos;
        [SerializeField] private Vector3 handAngle;
        #endregion

        #region MonoBehaviour Callbacks
        private void Update()
        {
            //上記のシリアライズフィールドにデータを代入
            isTracked = _hand.IsTracked;

            confidence = _hand.HandConfidence;

            isThumbPinch = _hand.GetFingerIsPinching(OVRHand.HandFinger.Thumb);
            ThumbPinchStrength = _hand.GetFingerPinchStrength(OVRHand.HandFinger.Thumb);

            isIndexPinch = _hand.GetFingerIsPinching(OVRHand.HandFinger.Index);
            IndexPinchStrength = _hand.GetFingerPinchStrength(OVRHand.HandFinger.Index);

            isMiddlePinch = _hand.GetFingerIsPinching(OVRHand.HandFinger.Middle);
            MiddlePinchStrength = _hand.GetFingerPinchStrength(OVRHand.HandFinger.Middle);

            isRingPinch = _hand.GetFingerIsPinching(OVRHand.HandFinger.Ring);
            RingPinchStrength = _hand.GetFingerPinchStrength(OVRHand.HandFinger.Ring);

            isPinkyPinch = _hand.GetFingerIsPinching(OVRHand.HandFinger.Pinky);
            PinkyPinchStrength = _hand.GetFingerPinchStrength(OVRHand.HandFinger.Pinky);

            if (!_hand.IsTracked) return;

            isThumbBend = HandTrackingUtility.IsBending(_hand, _skeleton, 0.95f, OVRSkeleton.BoneId.Hand_Thumb2, OVRSkeleton.BoneId.Hand_Thumb3, OVRSkeleton.BoneId.Hand_ThumbTip);
            isIndexBend = HandTrackingUtility.IsBending(_hand, _skeleton, 0.9f, OVRSkeleton.BoneId.Hand_Index1, OVRSkeleton.BoneId.Hand_Index2, OVRSkeleton.BoneId.Hand_Index3, OVRSkeleton.BoneId.Hand_IndexTip);
            isMiddleBend = HandTrackingUtility.IsBending(_hand, _skeleton, 0.9f, OVRSkeleton.BoneId.Hand_Middle1, OVRSkeleton.BoneId.Hand_Middle2, OVRSkeleton.BoneId.Hand_Middle3, OVRSkeleton.BoneId.Hand_MiddleTip);
            isRingBend = HandTrackingUtility.IsBending(_hand, _skeleton, 0.9f, OVRSkeleton.BoneId.Hand_Ring1, OVRSkeleton.BoneId.Hand_Ring2, OVRSkeleton.BoneId.Hand_Ring3, OVRSkeleton.BoneId.Hand_RingTip);
            isPinkyBend = HandTrackingUtility.IsBending(_hand, _skeleton, 0.9f, OVRSkeleton.BoneId.Hand_Pinky0, OVRSkeleton.BoneId.Hand_Pinky1, OVRSkeleton.BoneId.Hand_Pinky2, OVRSkeleton.BoneId.Hand_Pinky3, OVRSkeleton.BoneId.Hand_PinkyTip);

            //手の位置と手のひらの向きを線で描画(Sceneビューで確認可能)
            Debug.DrawLine(HandTrackingUtility.GetHandPosition(_skeleton), HandTrackingUtility.GetHandPosition(_skeleton) + HandTrackingUtility.GetHandForward(_skeleton), Color.red);
        }
        #endregion
    }
}
