using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.View.Penalty
{
    public class Obstacle : MonoBehaviour
    {
        [SerializeField] GameObject _targetObject;

        void Awake()
        {
            if (_targetObject == null)
            {
                Debug.LogError("TargetObject is null.");
            }
        }

        void OnCollisionEnter(Collision collisionInfo)
        {
            //他に軽量で正確な判定方法があればそちらに変更する
            //ReaperRobotの草の判定をレイヤーにして、こちらはタグにした方がいいか？
            if (collisionInfo.collider.gameObject == _targetObject 
                || collisionInfo.collider.gameObject.transform.parent.gameObject == _targetObject)
            {
                PenaltyManager.Instance.TriggerPenalty();
            }
        }
    }
}
