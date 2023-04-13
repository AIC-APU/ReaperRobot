using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.Localization.Settings;

namespace smart3tene.Reaper
{
    public class CutterCheckPoint : BaseCheckPoint
    {
        #region Public Fields
        public override string Introduction => _introduction;
        #endregion

        #region Enum
        private enum CutterGoal
        {
            ROTATE,
            STOP,
        }
        #endregion

        #region Serialized Private Fields
        [Header("Introduction")]
        [SerializeField, TextArea(1, 4)] private string _ja = "日本語の説明";
        [SerializeField, TextArea(1, 4)] private string _en = "Explanation in ENG";

        [Header("Setting")]
        [SerializeField] private CutterGoal _goal = CutterGoal.ROTATE;

       // [Header("ReaperManager")]
        //[SerializeField] private ReaperManager _reaperManager;
        #endregion

        #region Private Fields
        private TimeSpan CheckTime { get; set; }
        private IDisposable _disposable;
        private string _introduction = "haven't set introduction";
        #endregion

        #region Public method
        public override void SetUp()
        {
            // _disposable = _reaperManager.IsCutting.Subscribe(x =>
            // {
            //     if ((x && _goal == CutterGoal.ROTATE)
            //         || (!x && _goal == CutterGoal.STOP))
            //     {
            //         _isChecked.Value = true;
            //         OnChecked();
            //     }
            // });

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
            //ReaperEventManager.InvokeTextPopupEvent(_introduction);
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

