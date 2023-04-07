using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Localization.Settings;


namespace smart3tene.Reaper
{
    public class PerkingCheckPoint : BaseCheckPoint
    {
        #region Public Fields
        public override string Introduction => _introduction;
        #endregion

        #region Serialized Private Fields
        [Header("Introduction")]
        [SerializeField, TextArea(1, 4)] private string _ja = "日本語の説明";
        [SerializeField, TextArea(1, 4)] private string _en = "Explanation in ENG";

        [Header("Robot Object")]
        [SerializeField] private GameObject _robot;
        [SerializeField] private GameObject _goal;
        #endregion

        #region Private Fields
        private bool _isActive = false;
        private TimeSpan CheckTime { get; set; }
        private string _introduction = "haven't set introduction";
        #endregion

        #region Readonly Fields
        readonly float _distanceThreshold = 0.1f;
        readonly float _angleThreshold = 5f;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            _goal.SetActive(false);
        }
        private void Update()
        {
            if (!_isActive) return;

            var dis = Vector3.Distance(_robot.transform.position, _goal.transform.position);
            var angle = Vector3.Angle(_robot.transform.forward, _goal.transform.forward);

            if(dis < _distanceThreshold && angle < _angleThreshold)
            {
                _isChecked.Value = true;
                OnChecked();
            }
        }
        #endregion

        #region Public method
        public override void SetUp()
        {
            _goal.SetActive(true);
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
            _goal.SetActive(false);
            _isActive = false;
        }
        #endregion
    }

}
