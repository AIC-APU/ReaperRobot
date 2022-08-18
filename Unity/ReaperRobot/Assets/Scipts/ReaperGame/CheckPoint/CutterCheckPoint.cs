using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace smart3tene.Reaper
{
    public class CutterCheckPoint : BaseCheckPoint
    { 
        #region Public Fields
        public override string Introduction => _intro;
        #endregion

        #region Enum
        private enum CutterGoal
        {
            ROTATE,
            STOP,
        }
        #endregion

        #region Serialized Private Fields
        [SerializeField] private string _intro;
        [SerializeField] private CutterGoal _goal = CutterGoal.ROTATE;
        [SerializeField] private ReaperManager _reaperManager;
        #endregion

        #region Private Fields
        private TimeSpan CheckTime { get; set; }
        private IDisposable _disposable;
        #endregion

        #region Public method
        public override void SetUp()
        {
            _disposable = _reaperManager.IsCutting.Subscribe(x =>
            {
                if ((x && _goal == CutterGoal.ROTATE)
                    || (!x && _goal == CutterGoal.STOP))
                {
                    _isChecked.Value = true;
                    OnChecked();
                }
            });
        }

        public override void OnChecked()
        {
            CheckTime = GameTimer.GetCurrentTimeSpan;
            Debug.Log($"{name} is checked {CheckTime:hh\\:mm\\:ss}");
            _disposable?.Dispose();
        }
        #endregion
    }

}

