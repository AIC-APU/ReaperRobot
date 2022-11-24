using UniRx;

namespace smart3tene.Reaper
{
    public static class ViewMode
    {
        public enum ViewModeCategory
        {
            REAPER_FPV,
            REAPER_BIRDVIEW,
            REAPER_VR,
            REAPER_GAZE,
            REAPER_AROUND,
            PERSON_TPV,
        }

        public static ReactiveProperty<ViewModeCategory> NowViewMode { get; private set; } = new ReactiveProperty<ViewModeCategory>(ViewModeCategory.REAPER_FPV);

        public static bool IsLock { get;  set; } = false;

        public static void ChangeViewMode(ViewModeCategory viewMode)
        {
            if (IsLock)
            {
                UnityEngine.Debug.Log("ロックされているため変更できません。");
                return;
            }

            NowViewMode.Value = viewMode;
        }
    }

}
