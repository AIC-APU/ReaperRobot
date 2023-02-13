using UnityEngine;


namespace ReaperRobot.Scripts.UnityComponent.Camera
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
            Camera.transform.position = _target.transform.position + _cameraOffsetPos;
        }

        public override void ResetCamera()
        {
            _cameraOffsetPos = _target.TransformDirection(_cameraDefaultOffsetPos);
            _cameraOffsetRot = _cameraDefaultOffsetRot;
            Camera.transform.eulerAngles = _target.transform.eulerAngles + _cameraOffsetRot;
            Camera.fieldOfView = defaultFOV;
        }

        public override void MoveCamera(float horizontal, float vertical)
        {
            //vertical...ロボットに近づく
            var distance = Vector3.Distance(_target.transform.position, Camera.transform.position);
            if ((distance > zoomSpeed * 2f && vertical > 0) || (distance < zoomSpeed * 4f && vertical < 0))
            {
                Camera.transform.position += zoomSpeed * vertical * Camera.transform.forward;
            }

            //位置情報の更新
            _cameraOffsetPos = Camera.transform.position - _target.transform.position;
        }

        public override void RotateCamera(float horizontal, float vertical)
        {
            //位置情報の変更
            Camera.transform.position = _target.transform.position + _cameraOffsetPos;

            //horizontal...
            var horizontalAngle = horizontal * rotateSpeed;
            var center = new Vector3(_target.transform.position.x, Camera.transform.position.y, _target.transform.position.z);
            Camera.transform.RotateAround(center, Vector3.up, horizontalAngle);

            //vertical...
            var verticalAngle = vertical * rotateSpeed;
            if ((verticalAngle > 0 && Camera.transform.eulerAngles.x < maxAngleX)
                || (verticalAngle < 0 && Camera.transform.eulerAngles.x > minAngleX))
            {
                Camera.transform.RotateAround(_target.transform.position, Camera.transform.right, verticalAngle);
            }

            //位置情報の更新
            _cameraOffsetPos = Camera.transform.position - _target.transform.position;
        }
        #endregion

    }
}
