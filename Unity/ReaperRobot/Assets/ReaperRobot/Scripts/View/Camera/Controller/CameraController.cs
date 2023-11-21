using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Plusplus.ReaperRobot.Scripts.View.Camera
{
    [RequireComponent(typeof(PlayerInput))]
    public class CameraController : MonoBehaviour
    {
        #region Public Fields
        public BaseCamera ActiveCamera;
        #endregion

        #region  Serialized Private Fields
        [Header("Delay")]
        [SerializeField] private bool _enableDelay = false;
        [SerializeField] private float _delay = 0f;
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

            if (_actionMap["RotateCamera"].IsPressed())
            {
                var vec = _actionMap["RotateCamera"].ReadValue<Vector2>();
                RotateCamera(vec.x, vec.y);
            }
        }
        #endregion

        #region public method
        public void SwitchEnableDelay()
        {
            _enableDelay = !_enableDelay;
        }
        #endregion

        #region Private method
        private async void MoveCamera(InputAction.CallbackContext obj)
        {
            if (_enableDelay && _delay > 0) await UniTask.Delay((int)(_delay * 1000));
            var vec = obj.ReadValue<Vector2>();
            ActiveCamera.MoveCamera(vec.x, vec.y);
        }

        private async void ResetCamera(InputAction.CallbackContext obj)
        {
            if (_enableDelay && _delay > 0) await UniTask.Delay((int)(_delay * 1000));
            ActiveCamera.ResetCamera();
        }

        private async void RotateCamera(float horizontal, float vertical)
        {
            if (_enableDelay && _delay > 0) await UniTask.Delay((int)(_delay * 1000));
            ActiveCamera.RotateCamera(horizontal, vertical);
        }
        #endregion
    }
}