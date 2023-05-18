using UnityEngine;


namespace Plusplus.ReaperRobot.Scripts.View.Camera
{
    public class AroundViewCamera : BaseCamera
    {
        #region Private Fields
        private Vector3 _cameraOffsetPos;
        private Vector3 _cameraOffsetRot;
        #endregion

        #region Readonly Fields
        readonly float defaultFOV = 60f;
        readonly float zoomSpeed = 1f;
        readonly float rotateSpeed = 0.7f;
        readonly float minAngleX = 5f;
        readonly float maxAngleX = 50f;
        #endregion

        #region Public method
        public override void FollowTarget()
        {
            _camera.transform.position = _target.transform.position + _cameraOffsetPos;
        }

        public override void ResetCamera()
        {
            _cameraOffsetPos = _target.transform.TransformDirection(_cameraDefaultOffsetPos);
            _cameraOffsetRot = _cameraDefaultOffsetRot;
            _camera.transform.eulerAngles = _target.transform.eulerAngles + _cameraOffsetRot;
            _camera.fieldOfView = defaultFOV;
        }

        public override void MoveCamera(float horizontal, float vertical)
        {
            //vertical...ロボットに近づく
            var distance = Vector3.Distance(_target.transform.position, _camera.transform.position);
            if ((distance > zoomSpeed * 2f && vertical > 0) || (distance < zoomSpeed * 4f && vertical < 0))
            {
                _camera.transform.position += zoomSpeed * vertical * _camera.transform.forward;
            }

            //位置情報の更新
            _cameraOffsetPos = _camera.transform.position - _target.transform.position;
        }

        public override void RotateCamera(float horizontal, float vertical)
        {
            //位置情報の変更
            _camera.transform.position = _target.transform.position + _cameraOffsetPos;

            //horizontal...
            var horizontalAngle = horizontal * rotateSpeed;
            var center = new Vector3(_target.transform.position.x, _camera.transform.position.y, _target.transform.position.z);
            _camera.transform.RotateAround(center, Vector3.up, horizontalAngle);

            //vertical...
            var verticalAngle = vertical * rotateSpeed;
            if ((verticalAngle > 0 && _camera.transform.eulerAngles.x < maxAngleX)
                || (verticalAngle < 0 && _camera.transform.eulerAngles.x > minAngleX))
            {
                _camera.transform.RotateAround(_target.transform.position, _camera.transform.right, verticalAngle);
            }

            //位置情報の更新
            _cameraOffsetPos = _camera.transform.position - _target.transform.position;
        }
        #endregion

    }
}
