using UnityEngine;
using UnityEngine.InputSystem;

namespace smart3tene.Reaper
{
    [RequireComponent(typeof(PlayerInput))]
    public class ViewModeChanger : MonoBehaviour
    {
        #region Serialized Private Field
        [SerializeField] private ViewMode.ViewModeCategory _defaultViewMode;

        //以下で使うビューモードを選択してください
        [Header("Reaper")]
        [SerializeField] private bool _REAPER_FPV           = true;
        [SerializeField] private bool _REAPER_AROUND        = true;
        [SerializeField] private bool _REAPER_BIRDVIEW      = true;
        [SerializeField] private bool _REAPER_FromPERSON    = true;

        [Header("Person")]
        [SerializeField] private bool _PERSON_TPV           = true; 
        #endregion

        #region Private Fields
        private InputActionMap _reaperActionMap;
        private InputActionMap _personActionMap;

        private ViewMode.ViewModeCategory _lastViewMode = ViewMode.ViewModeCategory.REAPER_FPV;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            if (IsAllViewFalse())
            {
                Debug.LogError("1つ以上のビューを選択してください");
                Debug.Break();
            }

            ViewMode.ChangeViewMode(_defaultViewMode);

            _reaperActionMap = GetComponent<PlayerInput>().actions.FindActionMap("Reaper");
            _personActionMap = GetComponent<PlayerInput>().actions.FindActionMap("Person");

            //closeAppを登録
            _reaperActionMap["ChangeMode"].started += ChangeViewMode;
            _personActionMap["ChangeMode"].started += ChangeViewMode;

            _reaperActionMap["ChangeReaperAndPerson"].started += ChangeReaperAndPerson;
            _personActionMap["ChangeReaperAndPerson"].started += ChangeReaperAndPerson;
        }

        private void OnDestroy()
        {
            _reaperActionMap["ChangeMode"].started -= ChangeViewMode;
            _personActionMap["ChangeMode"].started -= ChangeViewMode;

            _reaperActionMap["ChangeReaperAndPerson"].started -= ChangeReaperAndPerson;
            _personActionMap["ChangeReaperAndPerson"].started -= ChangeReaperAndPerson;
        }
        #endregion

        #region Private method
        private void ChangeViewMode(InputAction.CallbackContext obj)
        {
            var nextView = NextViewMode(ViewMode.NowViewMode.Value);
            ViewMode.ChangeViewMode(nextView);
        }

        private void ChangeReaperAndPerson(InputAction.CallbackContext obj)
        {
            if (!_PERSON_TPV) return;

            if (ViewMode.NowViewMode.Value != ViewMode.ViewModeCategory.PERSON_TPV)
            {
                _lastViewMode = ViewMode.NowViewMode.Value;
                ViewMode.ChangeViewMode(ViewMode.ViewModeCategory.PERSON_TPV);
            }
            else
            {
                ViewMode.ChangeViewMode(_lastViewMode);
            }
        }

        private ViewMode.ViewModeCategory NextViewMode(ViewMode.ViewModeCategory viewMode)
        {
            ViewMode.ViewModeCategory nextView = _defaultViewMode;

            switch (viewMode)
            {
                case ViewMode.ViewModeCategory.REAPER_FPV:
                    if (_REAPER_AROUND)
                    {
                        nextView = ViewMode.ViewModeCategory.REAPER_AROUND;
                    }
                    else
                    {
                        nextView = NextViewMode(ViewMode.ViewModeCategory.REAPER_AROUND);
                    }
                    break;

                case ViewMode.ViewModeCategory.REAPER_AROUND:
                    if (_REAPER_BIRDVIEW)
                    {
                        nextView = ViewMode.ViewModeCategory.REAPER_BIRDVIEW;
                    }
                    else
                    {
                        nextView = NextViewMode(ViewMode.ViewModeCategory.REAPER_BIRDVIEW);
                    }
                    break;
                case ViewMode.ViewModeCategory.REAPER_BIRDVIEW:
                    if (_REAPER_FromPERSON)
                    {
                        nextView = ViewMode.ViewModeCategory.REAPER_FromPERSON;
                    }
                    else
                    {
                        nextView = NextViewMode(ViewMode.ViewModeCategory.REAPER_FromPERSON);
                    }
                    break;
                case ViewMode.ViewModeCategory.REAPER_FromPERSON:
                    if (_REAPER_FPV)
                    {
                        nextView = ViewMode.ViewModeCategory.REAPER_FPV;
                    }
                    else
                    {
                        nextView = NextViewMode(ViewMode.ViewModeCategory.REAPER_FPV);
                    }
                    break;

                default:
                    break;
            }

            return nextView;
        }

        private bool IsAllViewFalse()
        {
            return !_REAPER_FPV && !_REAPER_BIRDVIEW && !_REAPER_FromPERSON && !_REAPER_FPV && !_REAPER_AROUND && !_PERSON_TPV;
        }
        #endregion



    }
}

