using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.View.Camera
{
    public class TopCamera : BaseCamera
    {
        #region Readonly Fields
        readonly float zoomSpeed = 1f;
        readonly float rotateSpeed = 0.7f;
        readonly float maxHeight = 10f;
        readonly float minHeight = 1f;
        #endregion


        public override void FollowTarget()
        {
            _camera.transform.position = _target.transform.position + _target.transform.TransformDirection(_cameraDefaultOffsetPos);
            _camera.transform.rotation = _target.transform.rotation * Quaternion.Euler(_cameraDefaultOffsetRot);
        }

        public override void MoveCamera(float horizontal, float vertical)
        {
            var height = Mathf.Clamp(_cameraDefaultOffsetPos.y + vertical * zoomSpeed, minHeight, maxHeight);
            _cameraDefaultOffsetPos.y = height;
        }

        public override void RotateCamera(float horizontal, float vertical)
        {
            var rotateAngle = horizontal * rotateSpeed;
            _cameraDefaultOffsetRot.y += rotateAngle;
        }

        public override void ResetCamera()
        {
            _cameraDefaultOffsetPos.x = 0;
            _cameraDefaultOffsetPos.z = 0;

            _cameraDefaultOffsetRot.x = 90;
            _cameraDefaultOffsetRot.y = 0;
            _cameraDefaultOffsetRot.z = 0;
        }
    }
}
