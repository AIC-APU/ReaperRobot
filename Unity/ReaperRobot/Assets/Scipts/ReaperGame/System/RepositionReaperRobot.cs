using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace smart3tene.Reaper
{
    public class RepositionReaperRobot : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField] private ReaperManager _reaperManager;
        [SerializeField] private Transform _reaperTransform;

        [SerializeField] private Vector3 _defaultPos = new(0, 0, 0);
        [SerializeField] private Vector3 _defaultRot = new(0, 0, 0);
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            ReaperEventManager.ResetEvent += Reposition;
        }

        private void OnDisable()
        {
            ReaperEventManager.ResetEvent -= Reposition;
        }
        #endregion

         #region Private method
        private void Reposition()
        {
            _reaperManager.Move(0, 0);
            _reaperTransform.position = _defaultPos;
            _reaperTransform.rotation = Quaternion.Euler(_defaultRot);
            _reaperManager.MoveLift(true);
            _reaperManager.RotateCutter(true);
        }
        #endregion
    }
}

