using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.View.Camera
{
    public class CameraFollowActivater : MonoBehaviour
    {
        //CameraのFollowTargetのみを使用したい場合に使用する
        //CameraControllerとの併用はしないこと
        #region Public Fields
        [SerializeField] private BaseCamera _baseCamera;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            if(_baseCamera == null)
            {
                throw new System.NullReferenceException("Camera is null");
            }

            _baseCamera.ResetCamera();
        }
        void LateUpdate()
        {
            _baseCamera.FollowTarget();
        }
        #endregion
    }
}
