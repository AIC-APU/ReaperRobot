using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.View.Camera
{
    public class AroundViewCamera : BaseCamera
    {
        #region Serialized Fields
        [SerializeField] private bool _rotateCameraWhenTargetRotate = true;
        [SerializeField] private bool _lookAtTarget = true;
        #endregion

        #region Private Fields
        private Vector3 _cameraOffsetLocalPos;
        private Vector3 _cameraOffsetAngle;
        private Vector3 _cameraOffsetWorldPos;
        #endregion

        #region Readonly Fields
        readonly float defaultFOV = 60f;
        readonly float zoomSpeed = 1f;
        readonly float minDistance = 2f;
        readonly float maxDistance = 4f;
        readonly float rotateSpeed = 0.5f;
        readonly float minAngle = 30f;
        readonly float maxAngle = 70f;
        #endregion

        #region Public method
        public override void FollowTarget()
        {
            if (_rotateCameraWhenTargetRotate)
            {
                //ターゲットの回転に合わせてカメラが背後に回ってほしい場合はこっち（子オブジェクトの様にカメラが追従する）
                //ロボットに付けるカメラはこっちの方がいい
                _camera.transform.position = _target.transform.position + _target.transform.TransformDirection(_cameraOffsetLocalPos);
                _camera.transform.rotation = _target.transform.rotation * Quaternion.Euler(_cameraOffsetAngle);
            }
            else
            {
                //ターゲットの回転に合わせてカメラが回ってほしくない場合はこっち
                //キャラクターの様にカメラの向きによって移動方法を決めている場合はこっちの方がいい
                _camera.transform.position = _target.transform.position + _cameraOffsetWorldPos;
            }

            if (_lookAtTarget)
            {
                //カメラの向きをターゲットに向ける
                _camera.transform.LookAt(_target.transform.position);
            }
        }

        public override void ResetCamera()
        {
            //相対位置の初期化
            _cameraOffsetLocalPos = _cameraDefaultOffsetPos;
            _cameraOffsetWorldPos = _cameraDefaultOffsetPos;
            _cameraOffsetAngle = _cameraDefaultOffsetRot;

            //CameraのFOVを変更
            _camera.fieldOfView = defaultFOV;
        }

        public override void MoveCamera(float horizontal, float vertical)
        {
            //vertical...ロボットに近づく
            var distance = Vector3.Distance(_target.transform.position, _camera.transform.position);
            if ((distance > minDistance && vertical > 0) || (distance < maxDistance && vertical < 0))
            {
                _cameraOffsetLocalPos += zoomSpeed * vertical * _camera.transform.forward;
            }
        }

        public override void RotateCamera(float horizontal, float vertical)
        {
            //horizontal...
            var horizontalAngle = horizontal * rotateSpeed;
            _camera.transform.RotateAround(_target.transform.position, _target.transform.up, horizontalAngle);

            //vertical...
            var verticalAngle = vertical * rotateSpeed;
            var angle = Vector3.Angle(_target.transform.up, _camera.transform.position - _target.transform.position);
            if ((verticalAngle > 0 && angle > minAngle) || (verticalAngle < 0 && angle < maxAngle))
            {
                _camera.transform.RotateAround(_target.transform.position, _camera.transform.right, verticalAngle);
            }

            //相対位置の更新
            _cameraOffsetLocalPos = _target.transform.InverseTransformPoint(_camera.transform.position);
            _cameraOffsetWorldPos = _camera.transform.position - _target.transform.position;
            _cameraOffsetAngle = _camera.transform.eulerAngles - _target.transform.eulerAngles;
            _cameraOffsetAngle.x = Mathf.Repeat(_cameraOffsetAngle.x + 180f, 360f) - 180f;
        }
        #endregion
    }
}
