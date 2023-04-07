using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Localization.Settings;

namespace smart3tene.Reaper
{
    public class MoveCheckPoint :BaseCheckPoint
    {
        #region Public Property
        public override string Introduction => _introduction;
        #endregion

        #region Serialized Private Fields
        [Header("Introduction")]
        [SerializeField, TextArea(1, 4)] private string _ja = "日本語の説明";
        [SerializeField, TextArea(1, 4)] private string _en = "Explanation in ENG";

        [Header("Object")]
        [SerializeField] private GameObject _moveObject;
        #endregion

        #region Private Fields
        private TimeSpan CheckTime { get; set; }
        private Vector3 _firstPos;
        private bool _isActive = false;
        private string _introduction = "haven't set introduction";
        #endregion

        #region Monobehaviour Callbacks
        private void Update()
        {
            if (_isChecked.Value) return;
            if (!_isActive) return;

            var dis = Vector3.Distance(_moveObject.transform.position, _firstPos);
            if(dis > 1f)
            {
                _isChecked.Value = true;
                OnChecked();
            }
        }
        #endregion

        #region Public method
        public override void SetUp()
        {
            _firstPos = _moveObject.transform.position;
            _isActive = true;

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
            _isActive = false;
        }
        #endregion
    }

}
