using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

namespace smart3tene.Reaper
{
    public class PersonCameraController : MonoBehaviour, ICameraController
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
        private InputActionMap _personActionMap;
        private ReactiveProperty<bool> _isPersonMap = new(false);
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _personActionMap = _playerInput.actions.FindActionMap("Person");

            //インターフェースの取得
            if (_controllableCameraObject != null)
            {
                _controllableCamera = _controllableCameraObject.GetComponent<IControllableCamera>();
                _controllableCamera.ResetCamera();
            }

            _isPersonMap.Subscribe(x =>
            {
                if (x && CCamera != null)
                {
                    CCamera.ResetCamera();
                }
            });
        }

        private void Update()
        {
            _isPersonMap.Value = _playerInput.currentActionMap.name == "Person";
        }

        private void LateUpdate()
        {
            if (_playerInput.currentActionMap.name != "Person") return;

            //カメラの回転
            _controllableCamera.FollowTarget();

            var move = _personActionMap["Look"].ReadValue<Vector2>();
            _controllableCamera.RotateCamera(move.x, move.y);
        }
        #endregion
    }
}

