using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Photon.Pun;

namespace smart3tene.Reaper
{
    [RequireComponent(typeof(Rigidbody))]
    public class PersonManager : MonoBehaviourPun
    {
        #region Serialized Private Fields
        public Transform TPVCameraTransform;
        public Transform FPVCameraTransform;
        #endregion

        #region Private Fields
        private bool _isOperatable = true;
        private Rigidbody _rigidBody; 
        private Vector3 _tpvCameraOffsetPos;
        private float _cameraDistance;
        private Animator _animator;
        #endregion

        #region Readonly Fields
        readonly Vector3 fpvCameraOffsetPos = new Vector3(0, 1.5f, 0);
        readonly Vector3 defaultTPVCameraLocalPos = new(0, 2f, -2f);
        readonly Vector3 defaultTPVCameraEulerAngles = new(20, 0, 0);
        readonly float moveSpeed = 2f;
        readonly float rotateSpeed = 0.8f;
        #endregion

        #region MonoBehaviour Callbacks
        private void Start()
        {
            if (PhotonNetwork.IsConnected && !photonView.IsMine) return;
                 
            _rigidBody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();

            SetTPVCameraAtPlayerBack();

            _cameraDistance = Vector3.Distance(transform.position, TPVCameraTransform.position);

            if (GameSystem.Instance != null)
            {
                GameSystem.Instance.NowViewMode.Subscribe(x =>
                {
                    if(x == GameSystem.ViewMode.TPV)
                    {
                        _isOperatable = true;
                    }
                    else
                    {
                        //Move(0, 0);
                        //RotateTPVCamera(0, 0);
                        _isOperatable = false;
                    }
                });
            }
        }

        private void LateUpdate()
        {
            if (PhotonNetwork.IsConnected && !photonView.IsMine) return;

            //TPVCameraの位置
            TPVCameraTransform.position = transform.position + _tpvCameraOffsetPos;

            //FPVモードの時はPersonは草刈機を見る
            if (GameSystem.Instance != null && GameSystem.Instance.NowViewMode.Value == GameSystem.ViewMode.FPV)
            {
                transform.LookAt(GameSystem.Instance.ReaperInstance.transform);
                SetTPVCameraAtPlayerBack();
            }

            //FPVCameraの位置と角度
            FPVCameraTransform.position = transform.position + fpvCameraOffsetPos;
            FPVCameraTransform.forward = transform.forward;
        }
        #endregion

        #region Public Method
        public void Move(float horizontal, float vertical)
        {
            if (!_isOperatable) return;

            var cameraForward = Vector3.Scale(TPVCameraTransform.forward, new Vector3(1, 0, 1)).normalized;

            var moveForward = moveSpeed * vertical * cameraForward + moveSpeed * horizontal * TPVCameraTransform.right;

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
            var speed = new Vector2(_rigidBody.velocity.x, _rigidBody.velocity.z).magnitude;
            _animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
        }

        public void RotateTPVCamera(float horizontal, float vertical)
        {
            if (!_isOperatable) return;

            //回転の前にカメラの位置を更新しておく
            TPVCameraTransform.position = transform.position + _tpvCameraOffsetPos;

            //水平方向の回転
            var horizontalAngle = horizontal * rotateSpeed;
            TPVCameraTransform.RotateAround(transform.position, Vector3.up, horizontalAngle);

            //垂直方向の回転
            var verticalAngle = vertical * rotateSpeed;
            if((verticalAngle > 0 && TPVCameraTransform.eulerAngles.x < 60)
                || (verticalAngle < 0 && TPVCameraTransform.eulerAngles.x > 1))
            {
                TPVCameraTransform.RotateAround(transform.position, TPVCameraTransform.right, verticalAngle);
            }

            //位置情報の更新
            _tpvCameraOffsetPos = (TPVCameraTransform.position - transform.position).normalized * _cameraDistance;
        }

        private void SetTPVCameraAtPlayerBack()
        {
            _tpvCameraOffsetPos = transform.TransformVector(defaultTPVCameraLocalPos);
            TPVCameraTransform.position = transform.position + _tpvCameraOffsetPos;

            TPVCameraTransform.forward = transform.forward;
            TPVCameraTransform.eulerAngles += defaultTPVCameraEulerAngles;
        }
        #endregion

    }
}
