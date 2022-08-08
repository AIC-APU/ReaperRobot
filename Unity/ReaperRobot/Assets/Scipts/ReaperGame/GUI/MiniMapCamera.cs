using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace smart3tene.Reaper
{
    public class MiniMapCamera : MonoBehaviour
    {
        [SerializeField] private Camera _miniMapCamera;
        [SerializeField] private Transform _targetTransform;

        private void Awake()
        {
            if(_targetTransform == null)
            {
                _targetTransform = ReaperGameSystem.Instance.ReaperInstance.transform;
            }
        }

        private void LateUpdate()
        {
            if (_miniMapCamera == null || _targetTransform == null) return;

            _miniMapCamera.transform.position = new Vector3(_targetTransform.position.x, _miniMapCamera.transform.position.y, _targetTransform.position.z);
            _miniMapCamera.transform.eulerAngles = new Vector3(_miniMapCamera.transform.eulerAngles.x, _targetTransform.eulerAngles.y, _miniMapCamera.transform.eulerAngles.z);
        }
    }

}


