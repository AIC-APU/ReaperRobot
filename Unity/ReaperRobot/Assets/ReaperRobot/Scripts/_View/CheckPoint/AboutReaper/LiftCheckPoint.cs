using UnityEngine;
using UniRx;
using System;
using Plusplus.ReaperRobot.Scripts.View.ReaperRobot;

namespace Plusplus.ReaperRobot.Scripts.View.CheckPoint.AboutReaper
{
    public class LiftCheckPoint : BaseCheckPoint
    {
         #region Enum
        private enum LiftGoal
        {
            DOWN,
            UP,
        }
        #endregion

        #region Serialized Private Fields
        [Header("Setting")]
        [SerializeField] private LiftGoal _goal = LiftGoal.DOWN;
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
                .IsLiftDown
                .Subscribe(isDown =>
                {
                    if ((isDown && _goal == LiftGoal.DOWN)
                        || (!isDown && _goal == LiftGoal.UP))
                    {
                        _isChecked.Value = true;
                    }
                });
        }

        public override void FinalizeCheckPoint()
        {
            _disposable?.Dispose();
        }
        #endregion
    }
}
