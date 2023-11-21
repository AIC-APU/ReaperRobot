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
        private Vector3 _localPos;
        private Vector3 _localAngle;
        private Vector3 _positionOffset;
        #endregion

        #region Readonly Fields
        readonly float defaultFOV = 60f;
        readonly float zoomSpeed = 1f;
        readonly float minDistance = 2f;
        readonly float maxDistance = 4f;
        readonly float rotateSpeed = 0.7f;
        readonly float minAngle = 20f;
        readonly float maxAngle = 70f;
        #endregion

        #region Public method
        public override void FollowTarget()
        {
            if (_rotateCameraWhenTargetRotate)
            {
                //ターゲットの回転に合わせてカメラが背後に回ってほしい場合はこっち（子オブジェクトの様にカメラが追従する）
                //ロボットに付けるカメラはこっちの方がいい
                _camera.transform.position = _target.transform.TransformPoint(_localPos);
                _camera.transform.eulerAngles = _target.transform.eulerAngles + _localAngle;
            }
            else
            {
                //ターゲットの回転に合わせてカメラが回ってほしくない場合はこっち
                //キャラクターの様にカメラの向きによって移動方法を決めている場合はこっちの方がいい
                _camera.transform.position = _target.transform.position + _positionOffset;
            }

            if (_lookAtTarget)
            {
                //カメラの向きをターゲットに向ける
                _camera.transform.LookAt(_target.transform.position);
            }
        }

        public override void ResetCamera()
        {
            //Cameraの位置を変更
            _camera.transform.position = _target.transform.position + _target.transform.TransformDirection(_cameraDefaultOffsetPos);
            _camera.transform.eulerAngles = _target.transform.eulerAngles + _cameraDefaultOffsetRot;

            //相対位置の初期化
            SaveCameraLocalAndOffset(_camera, _target.transform);

            //CameraのFOVを変更
            _camera.fieldOfView = defaultFOV;
        }

        public override void MoveCamera(float horizontal, float vertical)
        {
            //vertical...ロボットに近づく
            var distance = Vector3.Distance(_target.transform.position, _camera.transform.position);
            if ((distance > minDistance && vertical > 0) || (distance < maxDistance && vertical < 0))
            {
                _camera.transform.position += zoomSpeed * vertical * _camera.transform.forward;
            }

            //相対位置の更新
            SaveCameraLocalAndOffset(_camera, _target.transform);
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
            SaveCameraLocalAndOffset(_camera, _target.transform);
        }
        #endregion

        #region Private method
        private void SaveCameraLocalAndOffset(UnityEngine.Camera camera, Transform targetTransform)
        {
            _localPos = targetTransform.InverseTransformPoint(camera.transform.position);

            _localAngle = camera.transform.eulerAngles - targetTransform.eulerAngles;
            _localAngle.x = Mathf.Repeat(_localAngle.x + 180f, 360f) - 180f;

            _positionOffset = camera.transform.position - targetTransform.position;
        }
        #endregion
    }
}
