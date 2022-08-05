using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace smart3tene.Reaper
{
    [RequireComponent(typeof(PlayerInput))]
    public class ActionMapSwicher : MonoBehaviour
    {
        //ViewModeに合わせてActionMapを変更します。

        #region Private Fields
        private PlayerInput _playerInput;
        #endregion

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();

            ViewMode.NowViewMode.Subscribe(mode =>
            {
                switch (mode)
                {
                    case ViewMode.ViewModeCategory.REAPER_FPV:
                    case ViewMode.ViewModeCategory.REAPER_BIRDVIEW:
                    case ViewMode.ViewModeCategory.REAPER_AROUND:
                    case ViewMode.ViewModeCategory.REAPER_FromPERSON:
                    case ViewMode.ViewModeCategory.REAPER_VR:
                        if (_playerInput.currentActionMap.name != "Reaper") _playerInput.SwitchCurrentActionMap("Reaper");
                        break;

                    case ViewMode.ViewModeCategory.PERSON_TPV:
                        if (_playerInput.currentActionMap.name != "Person") _playerInput.SwitchCurrentActionMap("Person");
                        break;

                    default:
                        break;
                }
            });
        }
    }

}
