using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

namespace smart3tene.Reaper
{
    public class ProjectorController : MonoBehaviour
    {
        [SerializeField] private CameraProjector _projector;
        [SerializeField] private Slider _delaySlider;
        [SerializeField] private TMP_Text _delayNumText;

        readonly int maxDelay = 3;

        private void Awake()
        {
            //sliderの初期化
            //このsliderから遅延を設定する
            InitializeDelaySlider();

            //robotFPVCameraをprojectorに設定
            _projector.recordingCamera = Camera.main;

            ViewMode.NowViewMode.Subscribe(mode =>
            {
                switch (mode)
                {
                    case ViewMode.ViewModeCategory.REAPER_FPV:
                    case ViewMode.ViewModeCategory.REAPER_AROUND:
                    case ViewMode.ViewModeCategory.REAPER_BIRDVIEW:
                        break;

                    default:
                        _delaySlider.value = 0f;
                        break;
                }
            }).AddTo(this);
        }

        //スライダーに設定する用の関数
        public void DelaySliderOnValueChaged()
        {
            _projector.delay = _delaySlider.value;
            _delayNumText.text = _delaySlider.value.ToString("F1");
        }

        private void InitializeDelaySlider()
        {
            _delaySlider.maxValue = maxDelay;
            _delaySlider.minValue = 0;

            _projector.delay = Mathf.Clamp(_projector.delay, 0, maxDelay);

            _delaySlider.value = _projector.delay;

            _delayNumText.text = _delaySlider.value.ToString("F1");
        }

    }


}
