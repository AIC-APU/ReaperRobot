using UnityEngine;
using TMPro;

namespace ReaperRobot.Scripts.UnityComponent.Camera
{
    public class CameraParamText : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private TMP_Text _positionX;
        [SerializeField] private TMP_Text _positionY;
        [SerializeField] private TMP_Text _positionZ;
        [SerializeField] private TMP_Text _rotationX;
        [SerializeField] private TMP_Text _rotationY;
        [SerializeField] private TMP_Text _rotationZ;
        #endregion

        #region MonoBehaviour Callbacks
        void Update()
        {
            var activeCamera = _cameraController.ActiveCamera.Value;

            var localPos = activeCamera.Target.InverseTransformPoint(activeCamera.Camera.transform.position);
            var localRot = GetLocalAngle(activeCamera.Target, activeCamera.Camera.transform);

            _positionX.text = localPos.x.ToString("F1");
            _positionY.text = localPos.y.ToString("F1");
            _positionZ.text = localPos.z.ToString("F1");
            _rotationX.text = localRot.x.ToString("F0");
            _rotationY.text = localRot.y.ToString("F0");
            _rotationZ.text = localRot.z.ToString("F0");
        }
        #endregion

        #region  Private Methods
        private Vector3 GetLocalAngle(Transform target, Transform camera)
        {
            var localX = NormalizeAngle(camera.eulerAngles.x - target.eulerAngles.x);
            var localY = NormalizeAngle(camera.eulerAngles.y - target.eulerAngles.y);
            var localZ = NormalizeAngle(camera.eulerAngles.z - target.eulerAngles.z);

            return new Vector3(localX, localY, localZ);

            float NormalizeAngle(float angle)
            {
                if (angle > 180)
                {
                    angle -= 360;
                }
                else if (angle < -180)
                {
                    angle += 360;
                }

                return angle;
            }
        }
        #endregion
    }
}