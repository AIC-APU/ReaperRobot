using Oculus.Interaction.PoseDetection.Debug;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchTest : MonoBehaviour
{
    #region Serialized Private Fields
    //インスペクタビューから現在の値を確認するためのシリアライズ
    [Header("Hand")]
    [SerializeField] private OVRHand _hand; //これはインスペクタビューから設定してほしい

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

    [Header("Pointer Pose")]
    [SerializeField] private Vector3 handPos;
    [SerializeField] private Vector3 handAngle;
    #endregion

    #region Private Fields
    //必要な変数あれば
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

        handPos = _hand.PointerPose.position;
        handAngle = _hand.PointerPose.eulerAngles;
    }
    #endregion
}
