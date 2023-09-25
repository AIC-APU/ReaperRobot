using UnityEngine;
using UniRx;
using System;
using Plusplus.ReaperRobot.Scripts.View.ReaperRobot;

namespace Plusplus.ReaperRobot.Scripts.View.CheckPoint.AboutReaper
{
    public class LiftCheckPoint : BaseCheckPoint
    {
        #region Serialized Private Fields
        [Header("Setting")]
        [SerializeField] private float _goal = 0.9f;
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
        public override void InitializeCheckPoint()
        {
            //CheckPointの判定
            _disposable =
                _reaperManager
                .LiftAngleRate
                .Where(rate => rate >= _goal)
                .Subscribe(_ => _isChecked.Value = true);
        }

        public override void FinalizeCheckPoint()
        {
            _disposable?.Dispose();
        }
        #endregion
    }
}
