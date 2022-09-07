using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace smart3tene.Reaper
{
    public class MainCamvas : MonoBehaviour
    {
        [SerializeField] private Canvas _mainCanvas;
        
        private void Awake()
        {
            //カメラの切り替え
            ViewMode.NowViewMode.Subscribe(mode =>
            {
                //画面切り替え、もっといい方法あればそうしたい
                switch (mode)
                {
                    case ViewMode.ViewModeCategory.REAPER_FPV:
                    case ViewMode.ViewModeCategory.REAPER_BIRDVIEW:
                    case ViewMode.ViewModeCategory.REAPER_AROUND:
                        _mainCanvas.enabled = true;
                        break;

                    case ViewMode.ViewModeCategory.REAPER_FromPERSON:
                    case ViewMode.ViewModeCategory.PERSON_TPV:
                        _mainCanvas.enabled = false;
                        break;

                    default:
                        break;
                }
            }).AddTo(this);
        }

        //以下ボタン用のメゾット
        public void ResetButtonClick()
        {
            ReaperEventManager.InvokeResetEvent();
        }

    }
}