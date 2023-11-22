using UnityEngine;
using UniRx;
using Plusplus.ReaperRobot.Scripts.View.ReaperRobot;

namespace Plusplus.ReaperRobot.Scripts.View.Replay
{
    public class LiftReplay : BaseReplay
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
            var liftData = _replayManager.GetLift();

            //後ほど、Recorder.cs の仕様が変わったから変更するかも。
            _reaperManager.MoveLift(liftData);
        }
        #endregion
    }
}
