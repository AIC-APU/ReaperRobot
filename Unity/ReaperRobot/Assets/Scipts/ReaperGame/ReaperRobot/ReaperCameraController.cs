using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

namespace smart3tene.Reaper
{
    [RequireComponent(typeof(PlayerInput))]
    public class ReaperCameraController : MonoBehaviour, ICameraController
    {
        public IControllableCamera CCamera 
        { 
            get => _controllableCamera; 
            set
            {
                _controllableCamera = value;
                _controllableCamera.ResetCamera();
            }
        }
        private IControllableCamera _controllableCamera;

        #region Serialized Private Fields
        [SerializeField, Tooltip("ここからIControllableCameraを設定することもできます（デバッグ用）")] private GameObject _controllableCameraObject;
        #endregion

        #region Private Fields
        private PlayerInput _playerInput;
        private InputActionMap _reaperActionMap;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _reaperActionMap = _playerInput.actions.FindActionMap("Reaper");

            if(_controllableCamera == null && _controllableCameraObject != null)
            {
                _controllableCamera = _controllableCameraObject.GetComponent<IControllableCamera>();
                _controllableCamera.ResetCamera();
            }

            _reaperActionMap["MoveCamera"].performed += MoveCamera;
            _reaperActionMap["ResetCamera"].started += ResetCamera;

            ReaperEventManager.ResetEvent += ResetCamera;
        }

        private void OnDisable()
        {
            _reaperActionMap["MoveCamera"].performed -= MoveCamera;
            _reaperActionMap["ResetCamera"].started -= ResetCamera;
        }

        private void LateUpdate()
        {
            if (!_playerInput.enabled || _playerInput.currentActionMap.name != "Reaper") return;
            if (_controllableCamera == null) return;

            _controllableCamera.FollowTarget();

            var vec = _reaperActionMap["RotateCamera"].ReadValue<Vector2>();
            _controllableCamera.RotateCamera(vec.x, vec.y);
        }

        private void OnDestroy()
        {
            ReaperEventManager.ResetEvent -= ResetCamera;
        }
        #endregion

        #region Private method
        private void MoveCamera(InputAction.CallbackContext obj)
        {
            var vec = obj.ReadValue<Vector2>();
            _controllableCamera.MoveCamera(vec.x, vec.y);
        }
        private void ResetCamera(InputAction.CallbackContext obj)
        {
            _controllableCamera.ResetCamera();
        }

        private void ResetCamera()
        {
            if (_playerInput.defaultActionMap != "Reaper") return;

            _controllableCamera.ResetCamera();
        }
        #endregion
    }
}

