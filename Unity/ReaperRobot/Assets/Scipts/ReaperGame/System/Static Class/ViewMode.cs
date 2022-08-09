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
            REAPER_FromPERSON,
            REAPER_AROUND,
            PERSON_TPV,
        }

        public static ReactiveProperty<ViewModeCategory> NowViewMode { get; private set; } = new ReactiveProperty<ViewModeCategory>(ViewModeCategory.REAPER_FPV);
    
        public static void ChangeViewMode(ViewModeCategory viewMode)
        {
            NowViewMode.Value = viewMode;
        }
    }

}
