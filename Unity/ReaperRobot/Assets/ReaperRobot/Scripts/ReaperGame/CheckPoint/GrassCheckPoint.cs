using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.Localization.Settings;

namespace smart3tene.Reaper
{
    public class GrassCheckPoint : BaseCheckPoint
    {
        #region Public Property
        public override string Introduction => _introduction;
        #endregion

        #region Serialized Private Fields
        [Header("Introduction")]
        [SerializeField, TextArea(1, 4)] private string _ja = "日本語の説明";
        [SerializeField, TextArea(1, 4)] private string _en = "Explanation in ENG";

        [Header("GoalRate")]
        [SerializeField] private int goalRate = 100;
        #endregion

        #region Private Fields
        private TimeSpan CheckTime{ get; set; }
        private IDisposable _disposable;
        private string _introduction = "haven't set introduction";
        #endregion

        #region Public method
        public override void SetUp()
        {
            // if(GrassCounter.AllGrassCount.Value == 0)
            // {
            //     Debug.LogError("草がありません");
            //     _isChecked.Value = true;
            //     OnChecked();
            // }

            // _disposable = GrassCounter.CutGrassPercent.Subscribe(x =>
            // {
            //     if (x >= goalRate)
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
            _disposable?.Dispose();
        }
        #endregion
    }
}