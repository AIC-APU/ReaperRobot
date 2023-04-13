using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.Localization.Settings;

namespace smart3tene.Reaper
{
    public class LiftCheckPoint : BaseCheckPoint
    {
        #region Public Property Fields
        public override string Introduction => _introduction;
        #endregion

        #region Enum
        private enum LiftGoal
        {
            UP,
            DOWN,
        }
        #endregion

        #region Serialized Private Field
        [Header("Introduction")]
        [SerializeField, TextArea(1, 4)] private string _ja = "日本語の説明";
        [SerializeField, TextArea(1, 4)] private string _en = "Explanation in ENG";

        [Header("Setting")]
        [SerializeField] private LiftGoal _goal = LiftGoal.UP;

        //[Header("ReaperMangaer")]
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
            // _disposable = _reaperManager.IsLiftDown.Subscribe(x =>
            // {
            //     if((x && _goal == LiftGoal.DOWN) 
            //         ||(!x && _goal == LiftGoal.UP))
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
