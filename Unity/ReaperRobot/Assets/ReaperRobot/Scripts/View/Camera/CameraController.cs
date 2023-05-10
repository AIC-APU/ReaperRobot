using UnityEngine;
using UnityEngine.InputSystem;

namespace Plusplus.ReaperRobot.Scripts.View.Camera
{
    [RequireComponent(typeof(PlayerInput))]
    public class CameraController : MonoBehaviour
    {
        #region  Serialized Private Fields
        [SerializeField] private CameraManager _cameraManager;
        #endregion

        #region Private Fields
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

            _cameraManager.FollowTarget();

            var vec = _actionMap["RotateCamera"].ReadValue<Vector2>();
            _cameraManager.RotateCamera(vec.x, vec.y);
        }
        #endregion

        #region Private method
        private void MoveCamera(InputAction.CallbackContext obj)
        {
            var vec = obj.ReadValue<Vector2>();
           _cameraManager.MoveCamera(vec.x, vec.y);
        }

        private void ResetCamera(InputAction.CallbackContext obj)
        {
            _cameraManager.ResetCamera();
        }

        private void ChangeCamera(InputAction.CallbackContext obj)
        {
            _cameraManager.ChangeCamera();
        }

        private void ChangeLoop(InputAction.CallbackContext obj)
        {
            _cameraManager.ChangeLoop();
        }
        #endregion
    }
}