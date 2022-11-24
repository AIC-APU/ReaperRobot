using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace smart3tene.Reaper
{
    //マルチプレイなどでBaseCameraのTargetをインスペクタビューから設定できない時に使ってください
    [DefaultExecutionOrder(-1)]
    [RequireComponent(typeof(BaseCamera))]
    public class SetCameraTarget : MonoBehaviour
    {
        private enum TargetMode
        {
            Reaper,
            Person
        }
        [SerializeField] private TargetMode _target = TargetMode.Reaper;

        private BaseCamera _baseCamera;

        private void Awake()
        {
            _baseCamera = GetComponent<BaseCamera>();

            if(_target == TargetMode.Reaper)
            {
                _baseCamera.Target = InstanceHolder.Instance.ReaperInstance.transform;
            }
            else if(_target == TargetMode.Person)
            {
                _baseCamera.Target = InstanceHolder.Instance.PersonInstance.transform;
            }
        }
    }
}

