using TMPro;
using UniRx;
using UnityEngine;

namespace smart3tene.Reaper
{
    public class CSVRecorderParamUI : MonoBehaviour
    {
        #region Serialized Private Fields
        [Header("ReaperRecorder")]
        [SerializeField] private ReaperRecorder _reaperRecorder;

        [Header("UI")]
        [SerializeField] private GameObject _recordingPanel;
        [SerializeField] private GameObject _csvDataPanel;
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private TMP_Text _inputHText;
        [SerializeField] private TMP_Text _inputVText;
        [SerializeField] private TMP_Text _liftText;
        [SerializeField] private TMP_Text _cutterText;
        [SerializeField] private TMP_Text _posXText;
        [SerializeField] private TMP_Text _posYText;
        [SerializeField] private TMP_Text _posZText;
        [SerializeField] private TMP_Text _angleText;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            //パネルの表示・非表示
            _recordingPanel
                .ObserveEveryValueChanged(x => x.activeSelf)
                .Subscribe(x => _csvDataPanel.SetActive(x))
                .AddTo(this);

            //以下テキスト更新
            // _reaperRecorder.RecordingTime
            //     .Subscribe(x => _timeText.text = GameTimer.ConvertSecondsToString(x))
            //     .AddTo(this);

            _reaperRecorder.InputH
                .Subscribe(x => _inputHText.text = x.ToString())
                .AddTo(this);

            _reaperRecorder.InputV
                .Subscribe(x => _inputVText.text = x.ToString())
                .AddTo(this);

            _reaperRecorder.LiftInt
                .Subscribe(x => _liftText.text = (x == 1).ToString())
                .AddTo(this);

            _reaperRecorder.CutterInt
                .Subscribe(x => _cutterText.text = (x == 1).ToString())
                .AddTo(this);

            _reaperRecorder.PosX
                .Subscribe(x => _posXText.text = x.ToString())
                .AddTo(this);

            _reaperRecorder.PosY
                .Subscribe(x => _posYText.text = x.ToString())
                .AddTo(this);

            _reaperRecorder.PosZ
                .Subscribe(x => _posZText.text = x.ToString())
                .AddTo(this);

            _reaperRecorder.Angle
                .Subscribe(x => _angleText.text = x.ToString())
                .AddTo(this);
        }
        #endregion
    }
}