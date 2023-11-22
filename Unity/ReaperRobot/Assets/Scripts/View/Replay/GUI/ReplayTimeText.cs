using UnityEngine;
using TMPro;
using System;


namespace Plusplus.ReaperRobot.Scripts.View.Replay
{
    public class ReplayTimeText : MonoBehaviour
    {
        [SerializeField] private ReplayManager _replayManager;
        [SerializeField] private TMP_Text _timeNum;

        private void Update()
        {
            if (_replayManager.IsReplaying.Value || !(_replayManager.Time.Value == 0))
            {
                _timeNum.text = TimeSpan.FromSeconds(_replayManager.Time.Value).ToString(@"hh\:mm\:ss\:fff");
            }
            else
            {
                _timeNum.text = "--:--:--:---";
            }
        }
    }
}
