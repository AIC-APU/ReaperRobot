using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace smart3tene.Reaper
{
    public class RepositionReaperRobot : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField] private ReaperManager _reaperManager;
        [SerializeField] private Transform _reaperTransform;

        readonly Vector3 _defaultPos = new(0f, 0f, 0f);
        readonly private Vector3 _defaultRot = new(0f, 0f, 0f);
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
        private async void Reposition()
        {
            _reaperManager.Move(0, 0);

            await UniTask.Yield();

            _reaperTransform.SetPositionAndRotation(_defaultPos, Quaternion.Euler(_defaultRot));

            _reaperManager.MoveLift(true);
            _reaperManager.RotateCutter(true);
        }
        #endregion
    }
}

