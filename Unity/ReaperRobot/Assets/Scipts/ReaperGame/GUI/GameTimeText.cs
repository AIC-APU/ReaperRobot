using TMPro;
using UnityEngine;

namespace smart3tene.Reaper
{
    public class GameTimeText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timeNum;

        private void Update()
        {
            if(!GameTimer.IsTimerRunning && GameTimer.GetCurrentSeconds == 0)
            {
                _timeNum.text = "";
            }
            else
            {
                _timeNum.text = GameTimer.GetCurrentTimeSpan.ToString(@"hh\:mm\:ss");
            }
        }
    }
}

