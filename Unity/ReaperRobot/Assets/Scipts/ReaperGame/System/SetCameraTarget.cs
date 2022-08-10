using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace smart3tene.Reaper
{
    //マルチプレイなどでIControllableCameraのTargetをインスペクタビューから設定できない時に使ってください
    [DefaultExecutionOrder(-1)]
    [RequireComponent(typeof(IControllableCamera))]
    public class SetCameraTarget : MonoBehaviour
    {
        private enum TargetMode
        {
            Reaper,
            Person
        }
        [SerializeField] private TargetMode _target = TargetMode.Reaper;


        private IControllableCamera _CCamera;

        private void Awake()
        {
            _CCamera = GetComponent<IControllableCamera>();

            if(_target == TargetMode.Reaper)
            {
                _CCamera.Target = ReaperGameSystem.Instance.ReaperInstance.transform;
            }
            else if(_target == TargetMode.Person)
            {
                _CCamera.Target = ReaperGameSystem.Instance.PersonInstance.transform;
            }
        }
    }
}

