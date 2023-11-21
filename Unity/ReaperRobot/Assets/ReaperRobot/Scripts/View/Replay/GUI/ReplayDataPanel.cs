using UnityEngine;
using TMPro;
using UniRx;


namespace Plusplus.ReaperRobot.Scripts.View.Replay
{
    public class ReplayDataPanel : MonoBehaviour
    {
        #region Serialize Private Fields
        [SerializeField] private ReplayManager _replayManager;
        [SerializeField] private TMP_Text _timeNum;
        [SerializeField] private TMP_Text _inputH;
        [SerializeField] private TMP_Text _inputV;
        [SerializeField] private TMP_Text _lift;
        [SerializeField] private TMP_Text _cutter;
        [SerializeField] private TMP_Text _positionX;
        [SerializeField] private TMP_Text _positionY;
        [SerializeField] private TMP_Text _positionZ;
        [SerializeField] private TMP_Text _angleY;
        #endregion


        #region MonoBehaviour Callbacks
        void Update()
        {
            if (_replayManager.NowData != null)
            {
                _timeNum.text = _replayManager.NowData.StringTime;
                _inputH.text = _replayManager.NowData.InputHorizontal.ToString();
                _inputV.text = _replayManager.NowData.InputVertical.ToString();
                _lift.text = _replayManager.NowData.Lift.ToString();
                _cutter.text = _replayManager.NowData.Cutter.ToString();
                _positionX.text = _replayManager.NowData.PositionX.ToString();
                _positionY.text = _replayManager.NowData.PositionY.ToString();
                _positionZ.text = _replayManager.NowData.PositionZ.ToString();
                _angleY.text = _replayManager.NowData.AngleY.ToString();
            }
            else
            {
                _timeNum.text = "--:--:--:---";
                _inputH.text = "--";
                _inputV.text = "--";
                _lift.text = "--";
                _cutter.text = "--";
                _positionX.text = "--";
                _positionY.text = "--";
                _positionZ.text = "--";
                _angleY.text = "--";
            }
        }
        #endregion
    }
}
