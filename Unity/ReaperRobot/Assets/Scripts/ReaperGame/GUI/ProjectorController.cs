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
        }

        //スライダーに設定する用の関数
        public void DelaySliderOnValueChaged()
        {
            SetDelay(_delaySlider.value);
        }

        private void InitializeDelaySlider()
        {
            _delaySlider.maxValue = maxDelay;
            _delaySlider.minValue = 0;

            var defaultDelay = Mathf.Clamp(_projector.delay, 0, maxDelay);
            SetDelay(defaultDelay);
            _delaySlider.value = defaultDelay;
        }

        private void SetDelay(float delay)
        {
            _projector.delay = delay;
            _delayNumText.text = delay.ToString("F1");
        }
    }
}
