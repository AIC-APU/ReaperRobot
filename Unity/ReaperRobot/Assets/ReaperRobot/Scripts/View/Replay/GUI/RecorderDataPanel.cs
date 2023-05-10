using UnityEngine;
using TMPro;
using UniRx;

namespace Plusplus.ReaperRobot.Scripts.View.Replay
{
    public class RecorderDataPanel : MonoBehaviour
    {
        #region Serialize Private Fields
        [SerializeField] private Recorder _recorder;
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
        void Start()
        {
            _recorder
                .NowData
                .Subscribe(x =>
                {
                    if (x != null)
                    {
                        _timeNum.text = x.StringTime;
                        _inputH.text = x.InputHorizontal.ToString();
                        _inputV.text = x.InputVertical.ToString();
                        _lift.text = x.Lift.ToString();
                        _cutter.text = x.Cutter.ToString();
                        _positionX.text = x.PositionX.ToString();
                        _positionY.text = x.PositionY.ToString();
                        _positionZ.text = x.PositionZ.ToString();
                        _angleY.text = x.AngleY.ToString();
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

                })
                .AddTo(this);
        }
        #endregion
    }
}
