using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace smart3tene.Reaper
{
    public class MoveCheckPoint :BaseCheckPoint
    {
        #region Public Property
        public override string Introduction => _intro;
        #endregion

        #region Serialized Private Fields
        [SerializeField] private string _intro = "Use WASD Key to move robot.";
        [SerializeField] private GameObject _moveObject;

        #endregion

        #region Private Fields
        private TimeSpan CheckTime { get; set; }
        private Vector3 _firstPos;
        private bool _isActive = false;
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
        }

        public override void OnChecked()
        {
            CheckTime = GameTimer.GetCurrentTimeSpan;
            Debug.Log($"{name} is checked {CheckTime:hh\\:mm\\:ss}");
            _isActive = false;
        }
        #endregion
    }

}
