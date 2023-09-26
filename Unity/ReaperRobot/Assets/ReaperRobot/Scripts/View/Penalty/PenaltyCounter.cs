using UniRx;
using UnityEngine;
using Cysharp.Threading.Tasks; 

namespace Plusplus.ReaperRobot.Scripts.View.Penalty
{
    public class PenaltyCounter : MonoBehaviour
    {
        private ReactiveProperty<int> _penaltyCount = new(0);

        public IReadOnlyReactiveProperty<int> PenaltyCountRx => _penaltyCount;
        public int PenaltyCount => _penaltyCount.Value;

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