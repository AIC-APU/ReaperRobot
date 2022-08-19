using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace smart3tene.Reaper
{
    public class ChangeViewModeCheckPoint : BaseCheckPoint
    {
        public override string Introduction => _intro;

        #region Serialized Private Fields
        [SerializeField] private string _intro;
        [SerializeField, Tooltip("View Modeが変更されただけでクリアとする")] private bool _justChange = true;
        [SerializeField] private ViewMode.ViewModeCategory _goal = ViewMode.ViewModeCategory.REAPER_FPV;
        #endregion

        #region Private Fields
        private TimeSpan CheckTime { get; set; }
        private IDisposable _disposable;
        #endregion

        #region Public method
        public override void SetUp()
        {
            _disposable = ViewMode.NowViewMode.Skip(1).Subscribe(viewmode => 
            {
                if(_justChange || viewmode == _goal)
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

