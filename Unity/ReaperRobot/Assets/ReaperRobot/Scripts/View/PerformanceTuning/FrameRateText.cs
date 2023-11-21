using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Plusplus.ReaperRobot.Scripts.View.PerformanceTuning
{
    public class FrameRateText : MonoBehaviour
    {
        [Header("Text")]
        [SerializeField] private TMP_Text _fpsNum;
        [SerializeField] private TMP_Text _label;

        [Header("Interval")]
        [SerializeField] private float _interval = 0.5f;

        [Header("Range")]
        [SerializeField] private float _lowFPS = 30f;
        [SerializeField] private float _highFPS = 60f;

        private float _timer = 0f;
        private int _frameCount = 0;

        void Update()
        {
            _timer += Time.deltaTime;
            _frameCount++;
            if (_timer >= _interval)
            {
                SetFPSText(_frameCount / _timer);

                _timer = 0f;
                _frameCount = 0;
            }
        }

        private void SetFPSText(float fps)
        {
            _fpsNum.text = fps.ToString("F2");

            if (fps <= _lowFPS)
            {
                _fpsNum.color = Color.red;
                _label.color = Color.red;
            }
            else if (fps <= _highFPS)
            {
                _fpsNum.color = Color.yellow;
                _label.color = Color.yellow;
            }
            else
            {
                _fpsNum.color = Color.white;
                _label.color = Color.white;
            }
        }
    }
}
