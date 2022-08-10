using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace smart3tene.Reaper
{
    public class CheckPoint : MonoBehaviour
    {
        #region Public Fields
        public IReadOnlyReactiveProperty<bool> IsChecked { get => _isChecked;}
        private ReactiveProperty<bool> _isChecked = new(false);

        public TimeSpan CheckTime { get; private set; }
        #endregion


        #region MonoBehaviour Callbacks
        private void OnTriggerEnter(Collider other)
        {
            if (_isChecked.Value) return;

            if (other.GetComponent<ReaperManager>())
            {   
                _isChecked.Value = true;
                CheckTime = GameTimer.GetCurrentTimeSpan;
                Debug.Log($"{name} is checked {CheckTime:hh\\:mm\\:ss}");
            }
        }
        #endregion
    }

}
