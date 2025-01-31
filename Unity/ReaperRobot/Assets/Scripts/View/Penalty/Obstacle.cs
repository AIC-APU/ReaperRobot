using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.View.Penalty
{
    public class PenaltyTrigger : MonoBehaviour
    {
        [SerializeField] PenaltyHandler _penaltyHandler;
        [SerializeField] GameObject _collisionTarget;

        void Awake()
        {
            if (_collisionTarget == null)
            {
                Debug.LogError("TargetObject is null.");
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject == _collisionTarget 
                || IsChildOfTarget(collision.gameObject, _collisionTarget))
            {
                _penaltyHandler.TriggerPenalty();
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
