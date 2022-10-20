using UnityEngine;
using UniRx;

namespace smart3tene.Reaper
{
    public class ViewModeLock : MonoBehaviour
    {
        [SerializeField] ViewMode.ViewModeCategory _lockCategory = ViewMode.ViewModeCategory.REAPER_FPV;

        private void Awake()
        {
            ViewMode.NowViewMode.Value = _lockCategory;
            ViewMode.IsLock = true;
        }
    }
}