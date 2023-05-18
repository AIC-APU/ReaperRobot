using UniRx;
using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.View.Mob
{
    public class PenaltyCounter : MonoBehaviour
    {
        public IReadOnlyReactiveProperty<int> PenaltyCount => _penaltyCount;
        private ReactiveProperty<int> _penaltyCount = new(0);

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            //PenaltyEvent += AddPenalty;
        }

        private void OnDestroy()
        {
            //PenaltyEvent -= AddPenalty;
        }
        #endregion

        #region Private method
        private void AddPenalty()
        {
            _penaltyCount.Value++;

            Debug.Log($"Penalty Count: {_penaltyCount.Value}");
        }
        #endregion
    }

}