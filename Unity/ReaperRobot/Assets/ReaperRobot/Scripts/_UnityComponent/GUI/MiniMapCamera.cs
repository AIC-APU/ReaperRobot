using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.UnityComponent.GUI
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class MiniMapCamera : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        private UnityEngine.Camera _miniMapCamera;

        void Awake()
        {
            _miniMapCamera = GetComponent<UnityEngine.Camera>();
        }

        private void LateUpdate()
        {
            try
            {
                _miniMapCamera.transform.position = new Vector3(_target.position.x, 
                                                                _miniMapCamera.transform.position.y, 
                                                                _target.position.z);

                _miniMapCamera.transform.eulerAngles = new Vector3(_miniMapCamera.transform.eulerAngles.x, 
                                                                    _target.eulerAngles.y, 
                                                                    _miniMapCamera.transform.eulerAngles.z);
            }
            catch(System.NullReferenceException e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}


