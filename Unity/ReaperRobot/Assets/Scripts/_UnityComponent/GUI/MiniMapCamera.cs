using UnityEngine;

namespace ReaperRobot.Scripts.UnityComponent.GUI
{
    public class MiniMapCamera : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Camera _miniMapCamera;
        [SerializeField] private Transform _targetTransform;

         private void LateUpdate()
        {
            if (_miniMapCamera == null || _targetTransform == null) return;

            _miniMapCamera.transform.position = new Vector3(_targetTransform.position.x, _miniMapCamera.transform.position.y, _targetTransform.position.z);
            _miniMapCamera.transform.eulerAngles = new Vector3(_miniMapCamera.transform.eulerAngles.x, _targetTransform.eulerAngles.y, _miniMapCamera.transform.eulerAngles.z);
        }
    }
}


