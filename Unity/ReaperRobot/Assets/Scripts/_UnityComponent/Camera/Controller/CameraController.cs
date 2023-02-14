using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

namespace ReaperRobot.Scripts.UnityComponent.Camera
{
    [RequireComponent(typeof(PlayerInput))]
    public class CameraController : MonoBehaviour
    {
        #region Struct
        [System.Serializable]
        public struct ViewLoop
        {
            public List<BaseCamera> Cameras;
        }
        [SerializeField] private List<ViewLoop> _loops = new List<ViewLoop>();
        #endregion

        #region  Public Fields
        public IReadOnlyReactiveProperty<BaseCamera> ActiveCamera => _activeCamera;
        #endregion

        #region Private Fields
        private ReactiveProperty<BaseCamera> _activeCamera = new();
        private int _cameraIndex = 0;
        private int _loopIndex = 0;
        private PlayerInput _playerInput;
        private InputActionMap _actionMap;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            //ActionMap"Camera"をアクティブに
            _playerInput = GetComponent<PlayerInput>();
            _actionMap = _playerInput.actions.FindActionMap("Camera");
            
            //Cameraアクションマップを有効化
            _actionMap.Enable();

            _actionMap["MoveCamera"].performed += MoveCamera;
            _actionMap["ResetCamera"].started += ResetCamera;
            _actionMap["ChangeCamera"].started += ChangeCamera;
            _actionMap["ChangeLoop"].started += ChangeLoop;

            if (_loops.Count == 0)
            {
                throw new System.NullReferenceException();
            }

            _activeCamera.Value = _loops[_loopIndex].Cameras[_cameraIndex];
            _activeCamera.Value.ResetCamera();
        }

        void OnDisable()
        {
            _actionMap["MoveCamera"].performed -= MoveCamera;
            _actionMap["ResetCamera"].started -= ResetCamera;
            _actionMap["ChangeCamera"].started -= ChangeCamera;
            _actionMap["ChangeLoop"].started -= ChangeLoop;
        }

        void LateUpdate()
        {
            //RotateCamera
            if (!_playerInput.enabled) return;
            if (_activeCamera == null) return;

            _activeCamera.Value.FollowTarget();

            var vec = _actionMap["RotateCamera"].ReadValue<Vector2>();
            _activeCamera.Value.RotateCamera(vec.x, vec.y);
        }
        #endregion

        #region Private method
        private void MoveCamera(InputAction.CallbackContext obj)
        {
            var vec = obj.ReadValue<Vector2>();
            _activeCamera.Value.MoveCamera(vec.x, vec.y);
        }

        private void ResetCamera(InputAction.CallbackContext obj)
        {
            _activeCamera.Value.ResetCamera();
        }

        private void ChangeCamera(InputAction.CallbackContext obj)
        {
            _cameraIndex++;
            if (_cameraIndex >= _loops[_loopIndex].Cameras.Count) _cameraIndex = 0;

            _activeCamera.Value = _loops[_loopIndex].Cameras[_cameraIndex];
            _activeCamera.Value.ResetCamera();
        }

        private void ChangeLoop(InputAction.CallbackContext obj)
        {
            _loopIndex++;
            if (_loopIndex >= _loops.Count) _loopIndex = 0;

            _cameraIndex = 0;

            _activeCamera.Value = _loops[_loopIndex].Cameras[_cameraIndex];
            _activeCamera.Value.ResetCamera();
        }
        #endregion
    }
}