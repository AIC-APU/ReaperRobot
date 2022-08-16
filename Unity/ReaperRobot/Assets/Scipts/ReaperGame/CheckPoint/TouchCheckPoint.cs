using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace smart3tene.Reaper
{
    public class TouchCheckPoint : BaseCheckPoint
    {
        #region private Fields
        private TimeSpan CheckTime { get; set; }
        #endregion


        #region MonoBehaviour Callbacks
        private void Awake()
        {
            _isChecked
                .Skip(1)
                .Subscribe(_ => OnChecked());

            gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!gameObject.activeSelf) return;
            if (_isChecked.Value) return;

            if (other.GetComponent<ReaperManager>())
            {   
                _isChecked.Value = true;
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
