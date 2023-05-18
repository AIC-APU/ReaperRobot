using UnityEngine;
using UniRx;
using Plusplus.ReaperRobot.Scripts.View.ReaperRobot;

namespace Plusplus.ReaperRobot.Scripts.View.Replay
{
    public class ShadowCutterReplay : BaseReplay
    {
        #region Serialized Private Fields
        [SerializeField] private ShadowReaperManager _shadowManager;
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
            _shadowManager.RotateCutter(cutterData);
        }
        #endregion
    }
}
