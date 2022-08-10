using UniRx;
using UnityEngine;

namespace smart3tene.Reaper
{
    public class ReaperCameraSwicher : MonoBehaviour
    {
        #region Serialized Private Fields
        [Header("Controller")]
        [SerializeField] private ReaperCameraController _reaperCameraController;

        [Header("Camera")]
        [SerializeField] private FPVCamera _reaperFPV = null;
        [SerializeField] private BirdViewCamera _reaperBirdView = null;
        [SerializeField] private AroundViewCamera _reaperAround = null;
        [SerializeField] private GazeCamera _reaperGaze = null;
        #endregion

        private void Awake()
        {
            ViewMode.NowViewMode.Subscribe(mode =>
            {
                switch (mode)
                {
                    case ViewMode.ViewModeCategory.REAPER_FPV:
                        if(_reaperFPV != null) _reaperCameraController.CCamera = _reaperFPV;
                        break;

                    case ViewMode.ViewModeCategory.REAPER_BIRDVIEW:
                        if (_reaperBirdView != null) _reaperCameraController.CCamera = _reaperBirdView;
                        break;

                    case ViewMode.ViewModeCategory.REAPER_AROUND:
                        if (_reaperAround != null) _reaperCameraController.CCamera = _reaperAround;
                        break;

                    case ViewMode.ViewModeCategory.REAPER_FromPERSON:
                        if (_reaperGaze != null) _reaperCameraController.CCamera = _reaperGaze;
                        break;

                    default:
                        break;
                }
            });
        }     
    }
}

