using UnityEngine;

namespace smart3tene
{
    public class BirdViewCamera : MonoBehaviour, IControllableCamera
    {
        #region Public Fields
        public Camera Camera { get => _camera; set => _camera = value; }
        [SerializeField] private Camera _camera;

        public Transform Target { get =>_target; set => _target = value; }
        [SerializeField] private Transform _target;
        #endregion

        #region Serialized Private Field
        [SerializeField] private Vector3 cameraDefaultOffsetPos = new(0f, 3f, -5f);
        [SerializeField] private Vector3 cameraDefaultOffsetRot = new(30f, 0f, 0f);
        #endregion

        #region Readonly Field
        readonly float _zoomSpeed = 1f;
        readonly float _rotateSpeed = 0.5f;
        readonly float _hightSpeed = 4f;
        readonly float _maxHeight = 20f;
        readonly float _minHeight = 0.5f;
        #endregion

        #region Public method
        /// <summary>
        /// LateUpdateなどで毎回呼ぶことで、カメラが追従する
        /// </summary>
        public void FollowTarget()
        {
            _camera.transform.LookAt(_target.transform.position);
        }

        public void ResetCamera()
        {
            _camera.transform.position = _target.transform.TransformPoint(cameraDefaultOffsetPos);
            _camera.transform.eulerAngles = _target.transform.eulerAngles + cameraDefaultOffsetRot;
            _camera.transform.LookAt(_target.transform.position);
        }

        public void MoveCamera(float horizontal, float vertical)
        {
            //vertical...ロボットに近づく
            var distance = Vector3.Distance(_target.transform.position, _camera.transform.position);
            if ((distance > _zoomSpeed * 2f && vertical > 0) || (distance < _zoomSpeed * 9f && vertical < 0))
            {
                _camera.transform.position += _zoomSpeed * vertical * _camera.transform.forward;

                var cameraPos = _camera.transform.position;
                cameraPos.y = Mathf.Clamp(cameraPos.y, _minHeight, _maxHeight);
                _camera.transform.position = cameraPos;
            }
        }

        public void RotateCamera(float horizontal, float vertical)
        {
            //vertical...高さを変更
            var cameraPos       = _camera.transform.position;      

            cameraPos.y += vertical * _hightSpeed * Time.deltaTime;
            cameraPos.y  = Mathf.Clamp(cameraPos.y, _minHeight, _maxHeight);
            _camera.transform.position = cameraPos;        

            //horizontal...gameobjectを中心に回転
            var center = new Vector3(_target.transform.position.x, _camera.transform.position.y, _target.transform.position.z);
            _camera.transform.RotateAround(center, Vector3.up,  -1 * horizontal * _rotateSpeed);
            _camera.transform.LookAt(_target.transform.position);
        }
        #endregion
    }

}
