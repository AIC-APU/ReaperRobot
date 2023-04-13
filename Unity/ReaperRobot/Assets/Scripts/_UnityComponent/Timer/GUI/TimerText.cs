using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ReaperRobot.Scripts.UnityComponent.Timer.GUI
{
    public class TimerText : MonoBehaviour
    {
        [SerializeField] private Timer _timer;
        [SerializeField] private TMP_Text _timeNum;

        private void Update()
        {
            if(_timer.IsTimerRunning || !(_timer.GetCurrentSeconds == 0))
            {
                _timeNum.text = _timer.GetCurrentTimeSpan.ToString(@"hh\:mm\:ss");
            }
            else
            {
                _timeNum.text = "--:--:--";
            }
        }
    }

}