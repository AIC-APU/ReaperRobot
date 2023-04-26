using System;
using System.Collections.Generic;
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

        #region Private Fields
        private IDisposable _disposable;
        #endregion

        #region MonoBehaviour Callbacks
        void OnDestroy()
        {
            _disposable?.Dispose();
        }
        #endregion

        #region Public method
        public override void FinalizeReplay()
        {
            _disposable?.Dispose();
            _dataSets.Clear();
        }

        public override void InitializeReplay(string filePath)
        {
            //データの読み込み
            _dataSets.Clear();
            _dataSets.AddRange(GetDataSets(filePath));

            _disposable =
                _timer
                .Time
                .Subscribe(seconds =>
                {
                    Replay(_dataSets, seconds);
                });
        }
        #endregion

        #region Private method
        protected override void Replay(List<DataSet> data, float seconds)
        {
            var cutterData = ExtractCutter(data, seconds);
            _reaperManager.RotateCutter(cutterData);
        }
        #endregion
    }
}
