using UnityEngine;
using TMPro;

namespace Plusplus.ReaperRobot.Scripts.View.Replay
{
    public class RecorderTimeText : MonoBehaviour
    {
        [SerializeField] private Recorder _recorder;
        [SerializeField] private TMP_Text _timeNum;

        private void Update()
        {
            if(_recorder.IsRecording.Value || !(_recorder.Time.Value == 0))
            {
                _timeNum.text = _recorder.NowData.Value.StringTime;
            }
            else
            {
                _timeNum.text = "--:--:--:---";
            }
        }
    }
}
