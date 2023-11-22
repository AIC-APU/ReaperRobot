using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using TMPro;

namespace Plusplus.ReaperRobot.Scripts.View.PerformanceTuning
{
    public class MemoryUsageText : MonoBehaviour
    {
        [Header("Text")]
        [SerializeField] private TMP_Text _memoryUsageNum;
        [SerializeField] private TMP_Text _label;

        [Header("Interval")]
        [SerializeField] private float _interval = 0.5f;


        private float _timer = 0f;


        void Update()
        {
            _timer += Time.unscaledDeltaTime;
            if(_timer >= _interval)
            {
                SetMemoryUsageText();
                _timer = 0f;
            }
        
        }

        private void SetMemoryUsageText()
        {
            var memoryUsage = Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024);
            _memoryUsageNum.text = memoryUsage.ToString("F1") + "MB";
        }
    }
}
