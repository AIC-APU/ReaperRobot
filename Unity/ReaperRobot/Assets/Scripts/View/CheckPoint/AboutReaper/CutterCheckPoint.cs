using UnityEngine;
using UniRx;
using System;
using Plusplus.ReaperRobot.Scripts.View.ReaperRobot;

namespace Plusplus.ReaperRobot.Scripts.View.CheckPoint.AboutReaper
{
    public class CutterCheckPoint : BaseCheckPoint
    {
        #region Enum
        private enum CutterGoal
        {
            ROTATE,
            STOP,
        }
        #endregion

        #region Serialized Private Fields
        [Header("Setting")]
        [SerializeField] private CutterGoal _goal = CutterGoal.ROTATE;
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
                .IsCutting
                .Subscribe(isRotating =>
                {
                    if ((isRotating && _goal == CutterGoal.ROTATE)
                        || (!isRotating && _goal == CutterGoal.STOP))
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
