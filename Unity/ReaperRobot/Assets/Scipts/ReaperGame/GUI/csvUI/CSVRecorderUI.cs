using Cysharp.Threading.Tasks;
using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace smart3tene.Reaper
{
    public class CSVRecorderUI : MonoBehaviour
    {
        #region Public Fields
        public bool ControllableRobot { get; private set; } = true;
        #endregion

        #region Serialized Private Fields
        [SerializeField] private ReaperRecorder _reaperRecorder;
        [SerializeField] private GameObject _recorderPanel;
        [SerializeField] private TMP_Text _recordingTimeText;
        [SerializeField] private Button _recordingButton;
        [SerializeField] private Button _stopButton;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            _recorderPanel.SetActive(false);

            _recordingButton.interactable = false;
            _stopButton.interactable = false;

            //TimeTextの更新
            _reaperRecorder.RecordingTime
                .Subscribe(x => _recordingTimeText.text = GameTimer.ConvertSecondsToString(x, false))
                .AddTo(this);
        }
        #endregion

        #region Public method
        public void OnClickRecord()
        {
            _reaperRecorder.StartRecording();

            _recordingButton.interactable = false;
            _stopButton.interactable = true;

            //コントローラの使用の許可
            ControllableRobot = true;
        }

        public void OnClickStop()
        {
            _reaperRecorder.StopRecording();

            _recorderPanel.SetActive(false);

            _recordingButton.interactable = false;
            _stopButton.interactable = false;
        }

        public void OnClickSelectRecordingMode()
        {
            if (_recorderPanel.activeSelf)
            {
                if (_reaperRecorder.IsRecording.Value)
                {
                    _reaperRecorder.StopRecording();
                }

                _recorderPanel.SetActive(false);

                _recordingButton.interactable = false;
                _stopButton.interactable = false;

                //コントローラの使用の許可
                ControllableRobot = true;
            }
            else
            {
                _recordingButton.interactable = true;
                _stopButton.interactable = false;

                _recorderPanel.SetActive(true);

                ReaperEventManager.InvokeResetEvent();

                //コントローラの使用の禁止
                ControllableRobot = false;
            }
        }
        #endregion
    }
}
