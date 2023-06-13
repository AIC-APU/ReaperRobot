using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.View.Camera
{
    public class CameraFollowActivater : MonoBehaviour
    {
        //CameraのFollowTargetのみを使用したい場合に使用する
        //CameraControllerとの併用はしないこと
        #region Public Fields
        [SerializeField] private BaseCamera _camera;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            if(_camera == null)
            {
                throw new System.NullReferenceException("Camera is null");
            }

            _camera.ResetCamera();
        }
        void LateUpdate()
        {
            _camera.FollowTarget();
        }
        #endregion
    }
}
