using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.UnityComponent.Camera
{
    [System.Serializable]
    public abstract class BaseCamera: MonoBehaviour
    {
        #region Public Fields
        public Transform Target => _target;
        public string ViewMode => _viewMode;
        public UnityEngine.Camera Camera => _camera;
        #endregion

        #region Protected Fields
        [SerializeField] protected string _viewMode = "TARGET_CAMERATYPE";
        [SerializeField] protected Transform _target;
        [SerializeField] protected UnityEngine.Camera _camera;
        [SerializeField] protected Vector3 _cameraDefaultOffsetPos = new(0f, 1.2f, -0.5f);
        [SerializeField] protected Vector3 _cameraDefaultOffsetRot = new(30f, 0f, 0f);
        #endregion

        #region Abstract Public Method
        public abstract void FollowTarget();
        public abstract void MoveCamera(float horizontal, float vertical);
        public abstract void RotateCamera(float horizontal, float vertical);
        public abstract void ResetCamera();
        #endregion
    }
}