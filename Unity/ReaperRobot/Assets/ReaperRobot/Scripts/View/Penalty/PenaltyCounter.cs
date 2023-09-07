using UniRx;
using UnityEngine;
using Cysharp.Threading.Tasks; 

namespace Plusplus.ReaperRobot.Scripts.View.Penalty
{
    public class PenaltyCounter : MonoBehaviour
    {
        public IReadOnlyReactiveProperty<int> PenaltyCount => _penaltyCount;
        private ReactiveProperty<int> _penaltyCount = new(0);

        #region MonoBehaviour Callbacks
        private async void Awake()
        {
            await UniTask.WaitUntil(() => PenaltyManager.Instance != null);
            PenaltyManager.Instance.PenaltyEvent += AddPenalty;
        }

        private void OnDestroy()
        {
            PenaltyManager.Instance.PenaltyEvent -= AddPenalty;
        }
        #endregion

        #region Private method
        private void AddPenalty()
        {
            _penaltyCount.Value++;
        }
        #endregion
    }

}