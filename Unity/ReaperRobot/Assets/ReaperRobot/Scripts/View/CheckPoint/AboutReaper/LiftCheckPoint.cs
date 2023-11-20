using UnityEngine;
using UniRx;
using System;
using Plusplus.ReaperRobot.Scripts.View.ReaperRobot;

namespace Plusplus.ReaperRobot.Scripts.View.CheckPoint.AboutReaper
{
    public class LiftCheckPoint : BaseCheckPoint
    {
        #region enum
        private enum LiftState
        {
            Down,
            Up
        }
        #endregion


        #region Serialized Private Fields
        [Header("Setting")]
        [SerializeField] private ReaperManager _reaperManager;
        [SerializeField] private float _goalLiftRate = 0.9f;
        [SerializeField] private LiftState _lifgGoal = LiftState.Up;
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
                .Where(rate => (_lifgGoal == LiftState.Down && rate < _goalLiftRate) || (_lifgGoal == LiftState.Up && rate > _goalLiftRate))
                .Subscribe(rate => _isChecked.Value = true);
        }

        public override void FinalizeCheckPoint()
        {
            _disposable?.Dispose();
        }
        #endregion
    }
}
