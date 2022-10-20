using UniRx;
using UnityEngine;

namespace smart3tene
{
    public class FPVCamera : MonoBehaviour, IControllableCamera
    {
        #region Public Fields
        public Camera Camera { get => _camera; set => _camera = value; }
        [SerializeField] private Camera _camera;

        public Transform Target { get => _target; set => _target = value; }
        [SerializeField] private Transform _target;
        #endregion

        #region Serialized Private Field
        [SerializeField] private Vector3 _cameraDefaultOffsetPos = new(0f, 1.2f, -0.5f);
        [SerializeField] private Vector3 _cameraDefaultOffsetRot = new(30f, 0f, 0f);
        #endregion

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
        public void FollowTarget()
        {
            _camera.transform.position = _target.transform.TransformPoint(_cameraOffsetPos.Value);
            _camera.transform.eulerAngles = _target.transform.eulerAngles + _cameraOffsetRot.Value;
        }

        public void ResetCamera()
        {
            _cameraOffsetPos.Value = _cameraDefaultOffsetPos;
            _cameraOffsetRot.Value = _cameraDefaultOffsetRot;
            _camera.fieldOfView = defaultFOV;
        }

        public void MoveCamera(float horizontal, float vertical)
        {
            var cameraSpeed = 0.1f;
            _cameraOffsetPos.Value += new Vector3(horizontal * cameraSpeed, vertical * cameraSpeed, 0);

            var clampedVec = _cameraOffsetPos.Value;

            clampedVec.x = Mathf.Clamp(clampedVec.x, -1f, 1f);
            clampedVec.y = Mathf.Clamp(clampedVec.y, 0.5f, 2f);
            clampedVec.z = Mathf.Clamp(clampedVec.z, -2f, 2f);

            _cameraOffsetPos.Value = clampedVec;
        }

        public void RotateCamera(float horizontal, float vertical)
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

