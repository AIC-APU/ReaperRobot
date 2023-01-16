using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.LookDev;

namespace smart3tene.Reaper
{
    [RequireComponent(typeof(PlayerInput))]
    public class ActionMapSwicher : MonoBehaviour
    {
        //ViewModeに合わせてActionMapを変更します。

        #region Private Fields
        private PlayerInput _playerInput;
        private string _lastActionMap;
        #endregion

        private void Start()
        {
            _playerInput = GetComponent<PlayerInput>();

            ViewMode
                .NowViewMode
                .Subscribe(mode =>
                {
                    switch (mode)
                    {
                        case ViewMode.ViewModeCategory.REAPER_FPV:
                        case ViewMode.ViewModeCategory.REAPER_BIRDVIEW:
                        case ViewMode.ViewModeCategory.REAPER_AROUND:
                        case ViewMode.ViewModeCategory.REAPER_GAZE:
                        case ViewMode.ViewModeCategory.REAPER_VR:
                            if (_playerInput.currentActionMap.name != "Reaper") _playerInput.SwitchCurrentActionMap("Reaper");
                            break;

                        case ViewMode.ViewModeCategory.PERSON_TPV:
                            if (_playerInput.currentActionMap.name != "Person") _playerInput.SwitchCurrentActionMap("Person");
                            break;

                        default:
                            break;
                    }

                    _playerInput.defaultActionMap = _playerInput.currentActionMap.name;
                })
                .AddTo(this);
        }
    }

}
