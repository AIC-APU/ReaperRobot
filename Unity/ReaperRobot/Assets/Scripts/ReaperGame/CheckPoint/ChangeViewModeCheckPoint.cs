using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.Localization.Settings;


namespace smart3tene.Reaper
{
    public class ChangeViewModeCheckPoint : BaseCheckPoint
    {
        public override string Introduction => _introduction;

        #region Serialized Private Fields
        [Header("Introduction")]
        [SerializeField, TextArea(1, 4)] private string _ja = "日本語の説明";
        [SerializeField, TextArea(1, 4)] private string _en = "Explanation in ENG";

        [Header("Setting")]
        [SerializeField, Tooltip("View Modeが変更されただけでクリアとする")] private bool _justChange = true;
        // [SerializeField] private ViewMode.ViewModeCategory _goal = ViewMode.ViewModeCategory.REAPER_FPV;
        #endregion

        #region Private Fields
        private TimeSpan CheckTime { get; set; }
        private IDisposable _disposable;
        private string _introduction = "haven't set introduction";
        #endregion

        #region Public method
        public override void SetUp()
        {
            // _disposable = ViewMode.NowViewMode.Skip(1).Subscribe(viewmode => 
            // {
            //     if(_justChange || viewmode == _goal)
            //     {
            //         _isChecked.Value = true;
            //         OnChecked();
            //     }
            // }).AddTo(this);

            //テキスト表示
            switch (LocalizationSettings.SelectedLocale.Identifier.Code)
            {
                case "ja":
                    _introduction = _ja;
                    break;
                case "en":
                    _introduction = _en;
                    break;
                default:
                    break;
            }
            ReaperEventManager.InvokeTextPopupEvent(_introduction);
        }

        protected override void OnChecked()
        {
            //CheckTime = GameTimer.GetCurrentTimeSpan;
            Debug.Log($"{name} is checked {CheckTime:hh\\:mm\\:ss}");
            _disposable?.Dispose();
        }
        #endregion
    }

}

