using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace smart3tene.Reaper
{
    public class LiftCheckPoint : BaseCheckPoint
    { 
        #region Public Property Fields
        public override string Introduction => _intro;
        #endregion

        #region Enum
        private enum LiftGoal
        {
            UP,
            DOWN,
        }
        #endregion

        #region Serialized Private Field
        [SerializeField] private string _intro;
        [SerializeField] private LiftGoal _goal = LiftGoal.UP;
        [SerializeField] private ReaperManager _reaperManager;
        #endregion

        #region Private Fields
        private TimeSpan CheckTime { get; set; }
        private IDisposable _disposable;
        #endregion

        #region Public method
        public override void SetUp()
        {
            _disposable = _reaperManager.IsLiftDown.Subscribe(x =>
            {
                if((x && _goal == LiftGoal.DOWN) 
                    ||(!x && _goal == LiftGoal.UP))
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
