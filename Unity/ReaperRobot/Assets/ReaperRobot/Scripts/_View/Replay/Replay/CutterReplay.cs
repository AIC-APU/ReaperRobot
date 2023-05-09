using UnityEngine;
using UniRx;
using Plusplus.ReaperRobot.Scripts.View.ReaperRobot;

namespace Plusplus.ReaperRobot.Scripts.View.Replay
{
    public class CutterReplay : BaseReplay
    {
        #region Serialized Private Fields
        [SerializeField] private ReaperManager _reaperManager;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            _replayManager
                .Time
                .Where(_ => _replayManager.IsDataReady)
                .Subscribe(_ =>
                {
                    Replay();
                })
                .AddTo(this);
        }
        #endregion

        #region Private method
        protected override void Replay()
        {
            var cutterData = _replayManager.GetCutter();
            _reaperManager.RotateCutter(cutterData);
        }
        #endregion
    }
}
