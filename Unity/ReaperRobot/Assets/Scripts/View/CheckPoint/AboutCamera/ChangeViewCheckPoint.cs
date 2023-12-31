using System;
using UniRx;
using UnityEngine;
using Plusplus.ReaperRobot.Scripts.View.Camera;

namespace Plusplus.ReaperRobot.Scripts.View.CheckPoint.AboutCamera
{
    public class ChangeViewCheckPoint : BaseCheckPoint
    {
        #region Serialized Private Fields
        [SerializeField] private CameraController _cameraController;
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
                                .Skip(1)
                                .Subscribe(_ => _isChecked.Value = true);
        }

        public override void FinalizeCheckPoint()
        {
            _disposable?.Dispose();
        }
        #endregion
    }
}
