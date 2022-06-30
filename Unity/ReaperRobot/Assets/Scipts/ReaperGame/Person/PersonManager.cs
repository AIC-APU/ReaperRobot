using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace smart3tene.Reaper
{
    [RequireComponent(typeof(Rigidbody))]
    public class PersonManager : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField] private Transform _TPVCamera;
        #endregion

        #region Private Fields
        private bool _isOperatable = true;
        private Rigidbody _rigidBody; 
        private Vector3 _cameraOffsetWorldPos = new(0, 2f, -2f);
        private float _cameraDistance;
        #endregion

        #region Readonly Fields
        readonly Vector3 defaultCameraEulerAngles = new(20, 0, 0);
        readonly float moveSpeed = 1.5f;
        readonly float rotateSpeed = 0.8f;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody>();

            _TPVCamera.transform.parent = null;

            _TPVCamera.position = transform.position + _cameraOffsetWorldPos;
            _TPVCamera.eulerAngles = defaultCameraEulerAngles;

            _cameraDistance = Vector3.Distance(transform.position, _TPVCamera.position);

            if (GameSystem.Instance != null)
            {
                GameSystem.Instance.NowOperationMode.Subscribe(x =>
                {
                    if(x == GameSystem.OperationMode.tpv)
                    {
                        _isOperatable = true;
                    }
                    else
                    {
                        Move(0, 0);
                        RotateCamera(0, 0);
                        _isOperatable = false;
                    }
                });
            }
        }

        private void LateUpdate()
        {
            _TPVCamera.position = transform.position + _cameraOffsetWorldPos;
            
            if(GameSystem.Instance != null && GameSystem.Instance.NowOperationMode.Value == GameSystem.OperationMode.fpv)
            {
                transform.LookAt(GameSystem.Instance.ReaperInstance.transform);
            }
        }
        #endregion

        #region Public Method
        public void Move(float horizontal, float vertical)
        {
            if (!_isOperatable) return;

            var cameraForward = Vector3.Scale(_TPVCamera.forward, new Vector3(1, 0, 1)).normalized;

            var moveForward = moveSpeed * vertical * cameraForward + moveSpeed * horizontal * _TPVCamera.right;

            _rigidBody.velocity = moveForward * moveSpeed + new Vector3(0, _rigidBody.velocity.y, 0);

            if(moveForward == Vector3.zero)
            {
                _rigidBody.angularVelocity = Vector3.zero;
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(moveForward, Vector3.up);
            }

            //必要ならここでアニメーションなどの操作

        }

        public void RotateCamera(float horizontal, float vertical)
        {
            if (!_isOperatable) return;

            //回転の前にカメラの位置を更新しておく
            _TPVCamera.position = transform.position + _cameraOffsetWorldPos;

            //水平方向の回転
            var horizontalAngle = horizontal * rotateSpeed;
            _TPVCamera.RotateAround(transform.position, Vector3.up, horizontalAngle);

            //垂直方向の回転
            var verticalAngle = vertical * rotateSpeed;
            if((verticalAngle > 0 && _TPVCamera.eulerAngles.x < 60)
                || (verticalAngle < 0 && _TPVCamera.eulerAngles.x > 1))
            {
                _TPVCamera.RotateAround(transform.position, _TPVCamera.right, verticalAngle);
            }

            //位置情報の更新
            _cameraOffsetWorldPos = (_TPVCamera.position - transform.position).normalized * _cameraDistance;
        }
        #endregion
    }
}
