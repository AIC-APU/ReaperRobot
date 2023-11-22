using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UniRx;

namespace Plusplus.ReaperRobot.Scripts.View.Camera
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(CameraController))]
    public class CameraSwitcher : MonoBehaviour
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

        #region Private Fields
        private CameraController _cameraController;
        private InputActionMap _actionMap;
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
            _cameraController = GetComponent<CameraController>();
            _cameraController.ActiveCamera = _loops[_loopIndex].Cameras[_cameraIndex];
            _cameraController.ActiveCamera.ResetCamera();

            //ActionMap"Camera"をアクティブに
            _actionMap = GetComponent<PlayerInput>().actions.FindActionMap("Camera");
            _actionMap["ChangeCamera"].started += ChangeCamera;
            _actionMap["ChangeLoop"].started += ChangeLoop;
        }
        
        void OnDisable()
        {
            _actionMap["ChangeCamera"].started -= ChangeCamera;
            _actionMap["ChangeLoop"].started -= ChangeLoop;
        }
        #endregion

        #region private method
        private void ChangeCamera(InputAction.CallbackContext obj)
        {
            _cameraIndex++;
            if (_cameraIndex >= _loops[_loopIndex].Cameras.Count)
            {
                _cameraIndex = 0;
            }

            _cameraController.ActiveCamera = _loops[_loopIndex].Cameras[_cameraIndex];
            _cameraController.ActiveCamera.ResetCamera();
        }

        private void ChangeLoop(InputAction.CallbackContext obj)
        {
            _loopIndex++;
            if (_loopIndex >= _loops.Count)
            {
                _loopIndex = 0;
            }

            _cameraIndex = 0;

            _cameraController.ActiveCamera = _loops[_loopIndex].Cameras[_cameraIndex];
            _cameraController.ActiveCamera.ResetCamera();
            
            OnChangeLoop.Invoke();
        }
        #endregion
    }
}
