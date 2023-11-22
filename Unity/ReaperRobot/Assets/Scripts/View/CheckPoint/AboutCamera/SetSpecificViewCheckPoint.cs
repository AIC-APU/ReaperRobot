using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UniRx;
using Plusplus.ReaperRobot.Scripts.View.Camera;

namespace Plusplus.ReaperRobot.Scripts.View.CheckPoint.AboutCamera
{
    public class SetSpecificViewCheckPoint : BaseCheckPoint
    {
        #region Serialized Private Fields
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private BaseCamera _goalCamera;
        #endregion

        #region Private Fields
        private IDisposable _disposable;
        #endregion

        #region MonoBehaviour Callbacks
        void OnDestroy()
        {
            _disposable?.Dispose();
        }
        #endregion

        #region Public method
        public override void InitializeCheckPoint()
        {
            _disposable = _cameraController
                                .ObserveEveryValueChanged(x => x.ActiveCamera)
                                .Where(x => x == _goalCamera)
                                .Subscribe(_ => _isChecked.Value = true);
        }

        public override void FinalizeCheckPoint()
        {
            _disposable?.Dispose();
        }
        #endregion
    }
}
