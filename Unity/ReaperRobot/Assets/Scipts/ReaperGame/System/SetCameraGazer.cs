using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace smart3tene.Reaper
{
    [RequireComponent(typeof(GazeCamera))]
    public class SetCameraGazer : MonoBehaviour
    {
        private GazeCamera _gazeCamera;

        private void Awake()
        {
            _gazeCamera = GetComponent<GazeCamera>();
            _gazeCamera.Gazer = ReaperGameSystem.Instance.PersonInstance.transform;
        }
    }

}
