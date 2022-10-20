using UnityEngine;


namespace smart3tene
{
    public class AroundViewCamera : MonoBehaviour, IControllableCamera
    {
        #region Public Fields
        public Camera Camera { get => _camera; set => _camera = value;}
        [SerializeField] private Camera _camera;

        public Transform Target { get => _target; set => _target = value; }
        [SerializeField] private Transform _target;
        #endregion

        #region Serialized Private Fields
        [SerializeField] private Vector3 _cameraDefaultOffsetPos = new(0f, 1f, -1.5f);
        [SerializeField] private Vector3 _cameraDefaultOffsetRot = new(30f, 0f, 0f);
        #endregion

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
        public void FollowTarget()
        {
            _camera.transform.position = _target.transform.position + _cameraOffsetPos;
        }

        public void ResetCamera()
        {
            _cameraOffsetPos = _target.TransformDirection(_cameraDefaultOffsetPos);
            _cameraOffsetRot = _cameraDefaultOffsetRot;
            _camera.transform.eulerAngles = _target.transform.eulerAngles + _cameraOffsetRot;
            _camera.fieldOfView = defaultFOV;
        }

        public void MoveCamera(float horizontal, float vertical)
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

        public void RotateCamera(float horizontal, float vertical)
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
