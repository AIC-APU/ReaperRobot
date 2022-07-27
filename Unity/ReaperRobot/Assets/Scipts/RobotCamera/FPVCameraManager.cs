using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Photon.Pun;

namespace smart3tene
{
    public class FPVCameraManager : MonoBehaviourPun, IRobotCamera
    {
        #region Public Fields
        public Camera Camera { get => _camera; set => _camera = value; }
        [SerializeField] private Camera _camera;
        #endregion

        #region Serialized Private Field
        [SerializeField] private  Vector3 cameraDefaultOffsetPos = new(0f, 1.2f, -0.5f);
        [SerializeField] private Vector3 cameraDefaultOffsetRot = new(30f, 0f, 0f);
        #endregion

        #region Private Fields
        public IReadOnlyReactiveProperty<Vector3> CameraOffsetPos => _cameraOffsetPos;
        private ReactiveProperty<Vector3> _cameraOffsetPos = new();

        public IReadOnlyReactiveProperty<Vector3> CameraOffsetRot => _cameraOffsetRot;
        private ReactiveProperty<Vector3> _cameraOffsetRot = new();
        #endregion

        #region MonoBehaviour Callbacks
        private void Start()
        {
            if (PhotonNetwork.IsConnected && !photonView.IsMine) return;

            ResetCamera();
            FollowRobot();
        }
        #endregion

        #region Public method
        /// <summary>
        /// LateUpdateなどで毎回呼ぶことで、カメラが追従する
        /// </summary>
        public void FollowRobot()
        {
            _camera.transform.position = transform.TransformPoint(_cameraOffsetPos.Value);
            _camera.transform.eulerAngles = transform.eulerAngles + _cameraOffsetRot.Value;
        }

        public void ResetCamera()
        {
            _cameraOffsetPos.Value = cameraDefaultOffsetPos;
            _cameraOffsetRot.Value = cameraDefaultOffsetRot;
        }

        public void MoveCamera(float horizontal, float vertical)
        {
            var cameraSpeed = 0.1f;
            _cameraOffsetPos.Value += new Vector3(horizontal * cameraSpeed, vertical * cameraSpeed, 0);

            var clampedVec = _cameraOffsetPos.Value;

            clampedVec.x = Mathf.Clamp(clampedVec.x, -1f, 1f);
            clampedVec.y = Mathf.Clamp(clampedVec.y, 0.5f, 2f);
            clampedVec.z = Mathf.Clamp(clampedVec.z, -2f, 2f);

            _cameraOffsetPos.Value = clampedVec;
        }

        public void RotateCamera(float horizontal, float vertical)
        {
            var rotateSpeed = 0.5f;
            _cameraOffsetRot.Value += new Vector3(-1 * vertical * rotateSpeed, horizontal * rotateSpeed, 0);

            var clampedVec = _cameraOffsetRot.Value;

            clampedVec.x = Mathf.Clamp(clampedVec.x, -90f, 90f);
            clampedVec.y = Mathf.Clamp(clampedVec.y, -90f, 90f);
            clampedVec.z = Mathf.Clamp(clampedVec.z, -90f, 90f);

            _cameraOffsetRot.Value = clampedVec;
        }
        #endregion
    }
}

