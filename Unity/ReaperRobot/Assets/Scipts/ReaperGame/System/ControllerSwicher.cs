using UniRx;
using UnityEngine;

namespace smart3tene.Reaper
{
    public class ControllerSwicher : MonoBehaviour
    {
        #region Serialized Private Fields
        [Header("Camera")]
        [SerializeField] private Camera _camera;

        [Header("Controllers")]
        [SerializeField] private ReaperController _reaperController;
        [SerializeField] private ReaperCameraController _reaperCameraController;
        [SerializeField] private PersonController _personController;
        [SerializeField] private PersonCameraController _personCameraController;

        [Header("Controller Camera")]
        [SerializeField] private FPVCamera _reaperFPV;
        [SerializeField] private BirdViewCamera _reaperBirdView;
        [SerializeField] private AroundViewCamera _reaperAround;
        [SerializeField] private GazeCamera _reaperGaze;
        [SerializeField] private AroundViewCamera _personAround;
        #endregion

        private GameObject _person;

        private void Awake()
        {
            SetUp(ReaperGameSystem.Instance.ReaperInstance, ReaperGameSystem.Instance.PersonInstance, _camera);
        }

        private void SetUp(GameObject reaper, GameObject person, Camera camera)
        {
            _person = person;

            _reaperFPV.Camera       = camera;
            _reaperBirdView.Camera  = camera;
            _reaperAround.Camera    = camera;
            _reaperGaze.Camera      = camera;
            _personAround.Camera    = camera;

            _reaperFPV.Target       = reaper.transform;
            _reaperBirdView.Target  = reaper.transform;
            _reaperAround.Target    = reaper.transform;
            _reaperGaze.Target      = reaper.transform;
            _personAround.Target    = person.transform;

            _reaperController.TargetRobot = reaper;
            _personController.Person = person;

            ViewMode.NowViewMode.Subscribe(mode =>
            {
                switch (mode)
                {
                    case ViewMode.ViewModeCategory.REAPER_FPV:
                        _reaperCameraController.CCamera = _reaperFPV;
                        break;

                    case ViewMode.ViewModeCategory.REAPER_BIRDVIEW:
                        _reaperCameraController.CCamera = _reaperBirdView;
                        break;

                    case ViewMode.ViewModeCategory.REAPER_AROUND:
                        _reaperCameraController.CCamera = _reaperAround;
                        break;

                    case ViewMode.ViewModeCategory.REAPER_FromPERSON:
                        _reaperGaze.Gazer = _person.transform;
                        _reaperCameraController.CCamera = _reaperGaze;
                        
                        break;

                    case ViewMode.ViewModeCategory.PERSON_TPV:
                        _personCameraController.CCamera = _personAround;
                        break;
                    default:
                        break;
                }
            });
        }
    }
}

