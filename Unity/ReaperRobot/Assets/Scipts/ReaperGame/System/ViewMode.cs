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

        private static ViewModeCategory _lastViewMode;

        public static void ChangeViewMode()
        {
            switch (NowViewMode.Value)
            {
                case ViewModeCategory.REAPER_FPV:
                    NowViewMode.Value = ViewModeCategory.REAPER_AROUND;
                    break;

                case ViewModeCategory.REAPER_AROUND:
                    NowViewMode.Value = ViewModeCategory.REAPER_BIRDVIEW;
                    break;

                case ViewModeCategory.REAPER_BIRDVIEW:
                    NowViewMode.Value = ViewModeCategory.REAPER_FromPERSON;
                    break;

                case ViewModeCategory.REAPER_FromPERSON:
                    NowViewMode.Value = ViewModeCategory.REAPER_FPV;
                    break;

                case ViewModeCategory.REAPER_VR:
                    //VRモードの時はモードを変えない
                    break;

                default:
                    break;
            }
        }

        public static void ChangeReaperAndPerson()
        {
            if (NowViewMode.Value != ViewModeCategory.PERSON_TPV)
            {
                _lastViewMode = NowViewMode.Value;
                NowViewMode.Value = ViewModeCategory.PERSON_TPV;
            }
            else
            {
                NowViewMode.Value = _lastViewMode;
            }
        }
    }

}
