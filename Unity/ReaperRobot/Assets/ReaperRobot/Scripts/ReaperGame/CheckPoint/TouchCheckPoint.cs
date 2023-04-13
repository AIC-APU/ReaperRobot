using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.Localization.Settings;

namespace smart3tene.Reaper
{
    public class TouchCheckPoint : BaseCheckPoint
    {
        public override string Introduction => _introduction;

        #region Serialized Private Fields
        [Header("Introduction")]
        [SerializeField, TextArea(1, 4)] private string _ja = "日本語の説明";
        [SerializeField, TextArea(1, 4)] private string _en = "Explanation in ENG";

        [Header("Reaper or Person Mode")]
        [SerializeField] private Mode _mode = Mode.Reaper;
        #endregion

        #region Enum
        private enum Mode
        {
            Person,
            Reaper,
        }
        #endregion

        #region private Fields
        private TimeSpan CheckTime { get; set; }
        private string _introduction = "haven't set introduction.";
        #endregion


        #region MonoBehaviour Callbacks
        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!gameObject.activeSelf) return;
            if (_isChecked.Value) return;

            // if ((_mode == Mode.Reaper && other.GetComponent<ReaperManager>())
            //     || (_mode == Mode.Person && other.GetComponent<PersonManager>()))
            // {   
            //     _isChecked.Value = true;
            //     OnChecked();
            // }
        }

        public override void SetUp()
        {
            if (_mode == Mode.Reaper)
            {
                GetComponent<Renderer>().material.color = new Color32(250, 50, 50, 70);
            }
            else if (_mode == Mode.Person)
            {
                GetComponent<Renderer>().material.color = new Color32(50, 250, 50, 70);
            }

            gameObject.SetActive(true);

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
            gameObject.SetActive(false);
        }
        #endregion
    }

}
