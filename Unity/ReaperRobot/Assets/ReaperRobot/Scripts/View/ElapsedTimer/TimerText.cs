using System;
using UnityEngine;
using TMPro;

namespace Plusplus.ReaperRobot.Scripts.View.ElapsedTimer
{
    public class TimerText : MonoBehaviour
    {
        [SerializeField] private ElapsedTimer _timer;
        [SerializeField] private TMP_Text _timeNum;

        private void Update()
        {
            if(_timer.IsTimerRunning || _timer.Time != 0)
            {
                _timeNum.text = TimeSpan.FromSeconds(_timer.Time).ToString(@"hh\:mm\:ss");
            }
            else
            {
                _timeNum.text = "--:--:--";
            }
        }
    }

}