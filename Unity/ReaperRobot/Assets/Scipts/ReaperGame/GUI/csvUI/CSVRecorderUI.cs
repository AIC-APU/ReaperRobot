using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace smart3tene.Reaper
{
    public class CSVRecorderUI : MonoBehaviour
    {
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
        }
        private void Update()
        {
            _recordingTimeText.text = GameTimer.ConvertSecondsToString(_reaperRecorder.RecordingTime, false);
        }
        #endregion

        #region Public method
        public void OnClickRecord()
        {
            _reaperRecorder.StartRecording();

            _recordingButton.interactable = false;
            _stopButton.interactable = true;
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
                if (_reaperRecorder.IsRecording)
                {
                    _reaperRecorder.StopRecording();
                }

                _recorderPanel.SetActive(false);

                _recordingButton.interactable = false;
                _stopButton.interactable = false;
            }
            else
            {
                _recordingButton.interactable = true;
                _stopButton.interactable = false;

                _recorderPanel.SetActive(true);
            }
        }
        #endregion
    }
}
