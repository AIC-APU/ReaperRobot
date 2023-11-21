using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.View.Replay
{
    public abstract class BaseReplay : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField] protected ReplayManager _replayManager;
        #endregion

        #region Abstract Method
        protected abstract void Replay();
        #endregion
    }
}
