using UniRx;
using UnityEngine;
using Cysharp.Threading.Tasks; 

namespace Plusplus.ReaperRobot.Scripts.View.Penalty
{
    public class PenaltyCounter : MonoBehaviour
    {
        [SerializeField] PenaltyHandler _penaltyHandler;
        private ReactiveProperty<int> _penaltyCount = new(0);

        public IReadOnlyReactiveProperty<int> PenaltyCountRx => _penaltyCount;
        public int PenaltyCount => _penaltyCount.Value;

        #region MonoBehaviour Callbacks
        void Awake()
        {
            _penaltyHandler.PenaltyEvent += AddPenalty;
        }

        private void OnDestroy()
        {
            _penaltyHandler.PenaltyEvent -= AddPenalty;
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