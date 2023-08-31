using UnityEngine;
using UnityEngine.InputSystem;

namespace Plusplus.ReaperRobot.Scripts.View.Camera
{
    [RequireComponent(typeof(PlayerInput))]
    public class CameraController : MonoBehaviour
    {
        #region  Serialized Private Fields
        public BaseCamera ActiveCamera;
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
        }

        void OnDisable()
        {
            _actionMap["MoveCamera"].performed -= MoveCamera;
            _actionMap["ResetCamera"].started -= ResetCamera;
        }

        void LateUpdate()
        {
            //RotateCamera
            if (!_playerInput.enabled) return;

            ActiveCamera.FollowTarget();

            if(_actionMap["RotateCamera"].IsPressed())
            {
                var vec = _actionMap["RotateCamera"].ReadValue<Vector2>();
                ActiveCamera.RotateCamera(vec.x, vec.y);
            }
        }
        #endregion

        #region Private method
        private void MoveCamera(InputAction.CallbackContext obj)
        {
            var vec = obj.ReadValue<Vector2>();
           ActiveCamera.MoveCamera(vec.x, vec.y);
        }

        private void ResetCamera(InputAction.CallbackContext obj)
        {
            ActiveCamera.ResetCamera();
        }
        #endregion
    }
}