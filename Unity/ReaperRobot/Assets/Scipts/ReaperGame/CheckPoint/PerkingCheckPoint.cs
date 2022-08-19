using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace smart3tene.Reaper
{
    public class PerkingCheckPoint : BaseCheckPoint
    {
        #region Public Fields
        public override string Introduction => _intro;
        #endregion

        #region Serialized Private Fields
        [SerializeField] private string _intro = "Place the robot to overlap the shadow.";
        [SerializeField] private GameObject _robot;
        [SerializeField] private GameObject _goal;
        #endregion

        #region Private Fields
        private bool _isActive = false;
        private TimeSpan CheckTime { get; set; }
        #endregion

        #region Readonly Fields
        readonly float _distanceThreshold = 0.2f;
        readonly float _angleThreshold = 10f;
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
        }
        public override void OnChecked()
        {
            _goal.SetActive(false);
            _isActive = false;

            CheckTime = GameTimer.GetCurrentTimeSpan;
            Debug.Log($"{name} is checked {CheckTime:hh\\:mm\\:ss}");
        }
        #endregion
    }

}
