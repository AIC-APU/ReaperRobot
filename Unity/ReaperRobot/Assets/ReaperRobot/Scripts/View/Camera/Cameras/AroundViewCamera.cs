using UnityEngine;


namespace Plusplus.ReaperRobot.Scripts.View.Camera
{
    public class AroundViewCamera : BaseCamera
    {
        #region Struct
        private struct CameraParam
        {
            public Vector3 angleOffset;
            public Vector3 positionOffset;
            public Vector3 localPos;
        }
        #endregion

        #region Serialized Fields
        [SerializeField] private bool _rotateCameraWhenTargetRotate = true;
        #endregion

        #region Private Fields
        private CameraParam _cameraParam;
        #endregion

        #region Readonly Fields
        readonly float defaultFOV = 60f;
        readonly float zoomSpeed = 1f;
        readonly float rotateSpeed = 0.7f;
        readonly float minAngleX = 5f;
        readonly float maxAngleX = 50f;
        readonly CameraParam _cameraDefaultParam;
        #endregion

        #region Public method
        public override void FollowTarget()
        {
            if (_rotateCameraWhenTargetRotate)
            {
                //ターゲットの回転に合わせてカメラが背後に回ってほしい場合はこっち（子オブジェクトの様にカメラが追従する）
                //ロボットに付けるカメラはこっちの方がいいかも
                _camera.transform.position = _target.transform.TransformPoint(_cameraParam.localPos);
                _camera.transform.eulerAngles = _target.transform.eulerAngles - _cameraParam.angleOffset;
            }
            else
            {
                //ターゲットの回転に合わせてカメラが回ってほしくない場合はこっち
                //キャラクターの様にカメラの向きによって移動方法を決めている場合はこっちの方がいい
                _camera.transform.position = _target.transform.position + _cameraParam.positionOffset;
            }
        }

        public override void ResetCamera()
        {
            //Cameraの位置を変更
            _camera.transform.position = _target.transform.position + _target.transform.TransformDirection(_cameraDefaultOffsetPos);
            _camera.transform.eulerAngles = _target.transform.eulerAngles + _cameraDefaultOffsetRot;

            //CameraParamの初期化
            _cameraParam = CalcCameraParam(_camera, _target);

            //CameraのFOVを変更
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
            _cameraParam = CalcCameraParam(_camera, _target);
        }

        public override void RotateCamera(float horizontal, float vertical)
        {
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
            _cameraParam = CalcCameraParam(_camera, _target);
        }
        #endregion

        #region Private method
        private CameraParam CalcCameraParam(UnityEngine.Camera camera, GameObject obj)
        {
            var param = new CameraParam();
            param.positionOffset = camera.transform.position - obj.transform.position;
            param.angleOffset = obj.transform.eulerAngles - camera.transform.eulerAngles;
            param.localPos = _target.transform.InverseTransformPoint(camera.transform.position);
            return param;
        }
        #endregion
    }
}
