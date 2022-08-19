using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace smart3tene.Reaper
{
    public class TouchCheckPoint : BaseCheckPoint
    {
        #region Public Property
        public override string Introduction => _introduction;
        [SerializeField] private string _introduction = "Go to Red Pole!";
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
        #endregion


        #region MonoBehaviour Callbacks
        private void Awake()
        {
            if(_mode == Mode.Reaper)
            {
                GetComponent<Renderer>().material.color = new Color32(250, 50, 50, 70);
            }
            else if(_mode == Mode.Person)
            {
                GetComponent<Renderer>().material.color = new Color32(50, 250, 50, 70);
            }

            gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!gameObject.activeSelf) return;
            if (_isChecked.Value) return;

            if ((_mode == Mode.Reaper && other.GetComponent<ReaperManager>())
                || (_mode == Mode.Person && other.GetComponent<PersonManager>()))
            {   
                _isChecked.Value = true;
                OnChecked();
            }
        }

        public override void SetUp()
        {
            gameObject.SetActive(true);
        }

        public override void OnChecked()
        {
            CheckTime = GameTimer.GetCurrentTimeSpan;
            Debug.Log($"{name} is checked {CheckTime:hh\\:mm\\:ss}");

            gameObject.SetActive(false);
        }
        #endregion
    }

}
