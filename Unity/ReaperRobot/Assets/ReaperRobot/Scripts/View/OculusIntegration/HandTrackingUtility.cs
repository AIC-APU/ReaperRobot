using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.View.OculusIntegration
{
    public static class HandTrackingUtility
    {
        /// <summary>
        /// 手の中心座標を取得
        /// </summary>
        public static Vector3 GetHandPosition(OVRSkeleton skeleton)
        {
            //人差し指の付け根、小指の付け根、手首の3座標の平均を手の座標として出力
            var index1_pos = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Index1].Transform.position;
            var pinky1_pos = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Pinky1].Transform.position;
            var wrist_pos = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_WristRoot].Transform.position;

            return (index1_pos + pinky1_pos + wrist_pos) / 3f;
        }

        /// <summary>
        /// 手の中心から手の平方向を示す単位ベクトルを取得
        /// </summary>
        public static Vector3 GetHandForward(OVRSkeleton skeleton)
        {
            var middle1_pos = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Middle1].Transform.position;
            var pinky1_pos = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Pinky1].Transform.position;
            var thumb1_pos = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Thumb1].Transform.position;

            var middleToPinky = pinky1_pos - middle1_pos;
            var middleToThumb = thumb1_pos - middle1_pos;

            var isLeft = skeleton.GetSkeletonType() == OVRSkeleton.SkeletonType.HandLeft;
            var handPalmVec = isLeft ? Vector3.Cross(middleToPinky, middleToThumb).normalized : Vector3.Cross(middleToThumb, middleToPinky).normalized;　//Unityは左手座標系なので、外積の順番はこれでいい

            return handPalmVec;
        }

        /// <summary>
        /// 指定した指が曲がっているかどうかを判定
        /// </summary>
        public static bool IsBending(OVRHand hand, OVRSkeleton skelton, float threshold, params OVRSkeleton.BoneId[] boneids)
        {
            //if (!hand.IsTracked) return false;
            if (boneids.Length < 3) return false;

            Vector3? oldVec = null;
            var dot = 1.0f;
            for (var index = 0; index < boneids.Length - 1; index++)
            {
                var v = (skelton.Bones[(int)boneids[index + 1]].Transform.position - skelton.Bones[(int)boneids[index]].Transform.position).normalized; 
                if (oldVec.HasValue)
                {
                    dot *= Vector3.Dot(v, oldVec.Value);
                }
                oldVec = v;
            }
            return dot < threshold;
        }

        /// <summary>
        /// 指定した指が伸びているかどうかを判定
        /// </summary>
        public static bool IsStraight(OVRHand hand, OVRSkeleton skelton, float threshold, params OVRSkeleton.BoneId[] boneids)
        {
            //if (!hand.IsTracked) return false;
            if (boneids.Length < 3) return false;

            return !IsBending(hand, skelton, threshold, boneids);
        }

        /// <summary>
        /// targetが手の平側にあるかどうかを判定
        /// </summary>
        public static bool IsObjectInPalm(OVRHand hand, OVRSkeleton skeleton, GameObject target)
        {
            //if (!hand.IsTracked) return false;

            var handPos = GetHandPosition(skeleton);
            var handToTarget = (target.transform.position - handPos).normalized;
            var handForward = GetHandForward(skeleton);

            var dot = Vector3.Dot(handForward, handToTarget);

            return dot > 0.5f;
        }

        /// <summary>
        /// 手の指が掴むジェスチャをしているかどうかを判定
        /// </summary>
        public static bool IsGrab(OVRHand hand, OVRSkeleton skelton, float thumbThreshold = 0.95f, float threshold = 0.4f)
        {
            //２つの閾値の値は経験測からの数字

            //各指の曲がり具合を調べる
            var isThumBend   = IsBending(hand, skelton, thumbThreshold, OVRSkeleton.BoneId.Hand_Thumb2, OVRSkeleton.BoneId.Hand_Thumb3, OVRSkeleton.BoneId.Hand_ThumbTip);
            var isIndexBend  = IsBending(hand, skelton, threshold, OVRSkeleton.BoneId.Hand_Index1, OVRSkeleton.BoneId.Hand_Index2, OVRSkeleton.BoneId.Hand_Index3, OVRSkeleton.BoneId.Hand_IndexTip);
            var isMiddleBend = IsBending(hand, skelton, threshold, OVRSkeleton.BoneId.Hand_Middle1, OVRSkeleton.BoneId.Hand_Middle2, OVRSkeleton.BoneId.Hand_Middle3, OVRSkeleton.BoneId.Hand_MiddleTip);
            var isRingBend   = IsBending(hand, skelton, threshold, OVRSkeleton.BoneId.Hand_Ring1, OVRSkeleton.BoneId.Hand_Ring2, OVRSkeleton.BoneId.Hand_Ring3, OVRSkeleton.BoneId.Hand_RingTip);
            var isPinkyend   = IsBending(hand, skelton, threshold, OVRSkeleton.BoneId.Hand_Pinky0, OVRSkeleton.BoneId.Hand_Pinky1, OVRSkeleton.BoneId.Hand_Pinky2, OVRSkeleton.BoneId.Hand_Pinky3, OVRSkeleton.BoneId.Hand_PinkyTip);

            //親指といずれかの指が曲がっていたらtrue
            return isThumBend && (isIndexBend || isMiddleBend || isRingBend || isPinkyend);
        }

    }
}