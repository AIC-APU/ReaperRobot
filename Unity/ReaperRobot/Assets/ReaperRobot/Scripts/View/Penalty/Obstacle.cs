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

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject == _targetObject 
                || IsChildOfTarget(collision.gameObject, _targetObject))
            {
                PenaltyManager.Instance.TriggerPenalty();
            }
        }

        private bool IsChildOfTarget(GameObject child, GameObject parent)
        {
            if (child.transform.parent == null)
            {
                return false;
            }
            else if (child.transform.parent.gameObject == parent)
            {
                return true;
            }
            else
            {
                return IsChildOfTarget(child.transform.parent.gameObject, parent);
            }
        }
    }
}
