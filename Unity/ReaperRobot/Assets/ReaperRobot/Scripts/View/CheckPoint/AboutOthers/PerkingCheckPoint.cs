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
        [SerializeField, Range(0,10)] private float _goalDistance = 0.1f;
        #endregion

        #region Private Fields
        private bool _isActive = false;
        #endregion

        #region MonoBehaviour Callbacks
        void Update()
        {
            if (_isActive && Vector3.Distance(_target.position, _goal.position) <= _goalDistance)
            {
                _isChecked.Value = true;
            }
        }
        #endregion

        #region Public method
        public override void InitializeCheckPoint()
        {
            _isActive = true;
        }

        public override void FinalizeCheckPoint()
        {
            _isActive = false;
        }
        #endregion
    }
}
