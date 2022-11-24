using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;
using UniRx.Triggers;

namespace smart3tene.Reaper
{
    public class PersonCameraController : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField] private BaseCamera _personCamera;
        #endregion

        #region Private Fields
        private PlayerInput _playerInput;
        private InputActionMap _personActionMap;
        private ReactiveProperty<bool> _isMapPerson = new(false);
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _personActionMap = _playerInput.actions.FindActionMap("Person");

            //ActionMapが切り替わったタイミングでカメラをリセットしたい
            _isMapPerson
                .Subscribe(x =>
                {
                    if (x && _personCamera != null)
                    {
                        _personCamera.ResetCamera();
                    }
                })
                .AddTo(this);

            ReaperEventManager.ResetEvent += ResetCamera;
        }

        private void Update()
        {
            if (!_playerInput.enabled) return;

            _isMapPerson.Value = _playerInput.currentActionMap.name == "Person";
        }

        private void LateUpdate()
        {
            if (!_playerInput.enabled || _playerInput.currentActionMap.name != "Person") return;

            //カメラの回転
            _personCamera.FollowTarget();

            var move = _personActionMap["Look"].ReadValue<Vector2>();
            _personCamera.RotateCamera(move.x, move.y);
        }

        private void OnDestroy()
        {
            ReaperEventManager.ResetEvent -= ResetCamera;
        }
        #endregion

        #region Private Method
        private void ResetCamera()
        {
            if (!_isMapPerson.Value) return;

            _personCamera.ResetCamera();
        }
        #endregion
    }
}

