using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

namespace smart3tene.Reaper
{
    public class GrassCheckPoint : BaseCheckPoint
    {
        #region Public Property
        public override string Introduction => _introduction;
        [SerializeField] private string _introduction = "Cut Grasses!";
        #endregion

        #region Serialized Private Fields
        [SerializeField] private int goalRate = 100;
        #endregion

        #region Private Fields
        private TimeSpan CheckTime{ get; set; }
        private IDisposable _disposable;
        #endregion

        #region Public method
        public override void SetUp()
        {
            if(GrassCounter.AllGrassCount.Value == 0)
            {
                Debug.LogError("草がありません");
                _isChecked.Value = true;
                OnChecked();
            }

            _disposable = GrassCounter.CutGrassPercent.Subscribe(x =>
            {
                if (x >= goalRate)
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