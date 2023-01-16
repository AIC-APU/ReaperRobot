using Oculus.Interaction.HandGrab;
using smart3tene.Reaper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace smart3tene
{
    [System.Serializable]
    public abstract class BaseCamera: MonoBehaviour
    {
        #region Public Fields
        public ViewMode.ViewModeCategory ViewMode => _viewMode;
        public Transform Target;
        #endregion

        #region Protected Fields
        [SerializeField] protected Camera Camera;
        [SerializeField] protected Vector3 _cameraDefaultOffsetPos = new(0f, 1.2f, -0.5f);
        [SerializeField] protected Vector3 _cameraDefaultOffsetRot = new(30f, 0f, 0f);
        [SerializeField] protected ViewMode.ViewModeCategory _viewMode;
        #endregion

        #region Abstract Public Method
        public abstract void FollowTarget();
        public abstract void MoveCamera(float horizontal, float vertical);
        public abstract void RotateCamera(float horizontal, float vertical);
        public abstract void ResetCamera();
        #endregion
    }

}