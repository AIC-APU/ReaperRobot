using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.View.CheckPoint
{
    public class PerkingCheckPoint : BaseCheckPoint
    {
        #region Serialized Private Fields
        [SerializeField] private Transform _target;
        [SerializeField] private Transform _goal;
        [SerializeField, Range(0.1f, 3.0f)] private float _goalDistance = 0.1f;
        [SerializeField, Range(1f, 30f)] private float _goalAngle = 10f;
        [SerializeField] private float _holdTime = 0.5f;
        #endregion

        #region Private Fields
        private bool _isActive = false;
        private float _timer = 0;
        #endregion

        #region MonoBehaviour Callbacks
        void Update()
        {
            if (_isActive 
                && Vector3.Distance(_target.position, _goal.position) <= _goalDistance
                && Vector3.Angle(_target.forward, _goal.forward) <= _goalAngle)
            {
                _timer += Time.deltaTime;
                if (_timer >= _holdTime)
                {
                    _isChecked.Value = true;
                }
            }
            else
            {
                _timer = 0;
            }
        }
        #endregion

        #region Public method
        public override void InitializeCheckPoint()
        {
            _isActive = true;
            _timer = 0;
        }

        public override void FinalizeCheckPoint()
        {
            _isActive = false;
        }
        #endregion
    }
}
