using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;

namespace Plusplus.ReaperRobot.Scripts.View.Camera
{
    public class CameraManager : MonoBehaviour
    {
        #region Struct
        [System.Serializable]
        public struct ViewLoop
        {
            public List<BaseCamera> Cameras;
        }
        #endregion

        #region  Serialized Private Fields
        [Header("Cameras")]
        [SerializeField] private List<ViewLoop> _loops = new List<ViewLoop>();
        #endregion

        #region Event
        [Header("Unity Events")]
        public UnityEvent OnChangeLoop;
        #endregion

         #region  Public Fields
        public IReadOnlyReactiveProperty<BaseCamera> ActiveCamera => _activeCamera;
        #endregion

        #region Private Fields
        private ReactiveProperty<BaseCamera> _activeCamera = new();
        private int _cameraIndex = 0;
        private int _loopIndex = 0;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
             if (_loops.Count == 0)
            {
                throw new System.NullReferenceException();
            }

            //初期カメラの設定
            _activeCamera.Value = _loops[_loopIndex].Cameras[_cameraIndex];
            _activeCamera.Value.ResetCamera();

            //カメラが変わる度にカメラのリセット
            _activeCamera
                .Subscribe(x => x.ResetCamera())
                .AddTo(this); 
        }
        #endregion

        #region Public method
        public void ChangeCamera()
        {
            _cameraIndex++;
            if (_cameraIndex >= _loops[_loopIndex].Cameras.Count)
            {
                _cameraIndex = 0;
            }

            _activeCamera.Value = _loops[_loopIndex].Cameras[_cameraIndex];
        }

        public void ChangeLoop()
        {
            _loopIndex++;
            if (_loopIndex >= _loops.Count)
            {
                _loopIndex = 0;
            }

            _cameraIndex = 0;

            _activeCamera.Value = _loops[_loopIndex].Cameras[_cameraIndex];

            OnChangeLoop.Invoke();
        }

        public void ResetCamera()
        {
            _activeCamera.Value.ResetCamera();
        }

        public void MoveCamera(float horizontal, float vertical)
        {
            _activeCamera.Value.MoveCamera(horizontal, vertical);
        }

        public void RotateCamera(float horizontal, float vertical)
        {
            _activeCamera.Value.RotateCamera(horizontal, vertical);
        }

        public void FollowTarget()
        {
            _activeCamera.Value.FollowTarget();
        }
        #endregion
    }
}
