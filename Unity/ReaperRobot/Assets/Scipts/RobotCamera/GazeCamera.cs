using UnityEngine;

namespace smart3tene.Reaper
{
    public class GazeCamera : MonoBehaviour, IControllableCamera
    {
        public Camera Camera { get => _camera; set => _camera = value; }
        [SerializeField] private Camera _camera;

        public Transform Target { get => _target; set => _target = value; }
        [SerializeField] private Transform _target;

        public Transform Gazer;

        #region Private Fields
        [SerializeField] private bool _rotateGazerY = true;
        #endregion

        #region Private Fields
        private Vector3 _targetOffset;
        private float _heigthOffset;
        private float _sideOffset;
        #endregion

        #region Readonly Fields
        readonly Vector3 gazerPosOffset = new Vector3(0, 1, 0);

        readonly float defaultFoV = 60f;
        readonly float maxFoV = 90f;
        readonly float minFoV = 20f;
        readonly float zoomSpeed = 10f;

        readonly float maxOffsetBase = 0.4f;
        readonly float rotateSpeed = 2f;
        #endregion

        #region Public Method
        public void FollowTarget()
        {
            //カメラが徐々にtarget + _targetOffsetの方を向く
            _camera.transform.LookAt(_target.transform.position + _targetOffset); //offset追加

            if (_rotateGazerY)
            {
                var pos = _target.transform.position + _targetOffset;
                pos.y = Gazer.position.y;
                Gazer.transform.LookAt(pos, Vector3.up);
            }
        }

        public void MoveCamera(float horizontal, float vertical)
        {
            var fov = _camera.fieldOfView;

            fov += -vertical * zoomSpeed;

            fov = Mathf.Clamp(fov, minFoV, maxFoV);

            _camera.fieldOfView = fov;
        }

        public void ResetCamera()
        {
            _camera.transform.position = Gazer.position + gazerPosOffset;
            _camera.transform.LookAt(_target.transform.position);
            _camera.fieldOfView = defaultFoV;

            _heigthOffset = 0;
            _sideOffset = 0;
            _targetOffset = Vector3.zero;
        }

        public void RotateCamera(float horizontal, float vertical)
        {
            var distance = Vector3.Distance(_camera.transform.position, _target.position);

            _heigthOffset += vertical * rotateSpeed * Time.deltaTime;
            _heigthOffset = Mathf.Clamp(_heigthOffset, -maxOffsetBase * distance, maxOffsetBase * distance);

            _sideOffset += horizontal * rotateSpeed * Time.deltaTime;
            _sideOffset = Mathf.Clamp(_sideOffset, -maxOffsetBase * distance, maxOffsetBase * distance);

            _targetOffset = new Vector3(0, _heigthOffset, 0) + _camera.transform.right * _sideOffset;
        }
        #endregion
    }
}

