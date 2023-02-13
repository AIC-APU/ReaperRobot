using UniRx;
using UnityEngine;

namespace ReaperRobot.Scripts.UnityComponent.Camera
{
    public class FPVCamera : BaseCamera
    {
        #region Private Fields
        public IReadOnlyReactiveProperty<Vector3> CameraOffsetPos => _cameraOffsetPos;
        private ReactiveProperty<Vector3> _cameraOffsetPos = new();
        public IReadOnlyReactiveProperty<Vector3> CameraOffsetRot => _cameraOffsetRot;
        private ReactiveProperty<Vector3> _cameraOffsetRot = new();
        #endregion

        #region Readonly Fields
        readonly float defaultFOV = 60f;
        #endregion

        #region Public method
        /// <summary>
        /// LateUpdateなどで毎回呼ぶことで、カメラが追従する
        /// </summary>
        public override void FollowTarget()
        {
            Camera.transform.position = _target.transform.TransformPoint(_cameraOffsetPos.Value);
            Camera.transform.eulerAngles = _target.transform.eulerAngles + _cameraOffsetRot.Value;
        }

        public override void ResetCamera()
        {
            _cameraOffsetPos.Value = _cameraDefaultOffsetPos;
            _cameraOffsetRot.Value = _cameraDefaultOffsetRot;
            Camera.fieldOfView = defaultFOV;
        }

        public override void MoveCamera(float horizontal, float vertical)
        {
            var cameraSpeed = 0.1f;
            _cameraOffsetPos.Value += new Vector3(horizontal * cameraSpeed, vertical * cameraSpeed, 0);

            var clampedVec = _cameraOffsetPos.Value;

            clampedVec.x = Mathf.Clamp(clampedVec.x, -1f, 1f);
            clampedVec.y = Mathf.Clamp(clampedVec.y, 0.5f, 2f);
            clampedVec.z = Mathf.Clamp(clampedVec.z, -2f, 2f);

            _cameraOffsetPos.Value = clampedVec;
        }

        public override void RotateCamera(float horizontal, float vertical)
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

