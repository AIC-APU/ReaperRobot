using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEngine.Rendering;

namespace smart3tene.Reaper 
{
    public class RecorderAndPlayerUI : MonoBehaviour
    {
        #region Serialized Private Fields
        [Header("CSV Player")]
        [SerializeField] private ShadowReaperPlayer _shadowReaperPlayer;
        [SerializeField] private RobotReaperPlayer _robotReaperPlayer;

        [Header("Panels")]
        [SerializeField] private GameObject _RecordingPanel;
        [SerializeField] private GameObject _playPanel;


        [Header("PlayPanel Parts")]
        [SerializeField] private TMP_Text _modeText;
        [SerializeField] private TMP_Text _fileNameText;
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _stopButton;

        [Header("Reaper Button")]
        [SerializeField] private Button _downLiftButton;
        [SerializeField] private Button _upLiftButton;
        [SerializeField] private Button _rotateCutterButton;
        [SerializeField] private Button _stopCutterButton;
        
        #endregion

        #region Private Fields
        enum PlayMode
        {
            Shadow,
            Robot,
        }
        private ReactiveProperty<PlayMode> _playMode = new(PlayMode.Shadow);

        private BaseCSVPlayer _csvPlayer;

        private bool _isPlayerActive = false;
        #endregion


        #region Readonly Field
        readonly string defaultFileDirectory = Path.GetFullPath(Application.streamingAssetsPath + "/../../../InputLog");
        readonly string defaultFileNameText = "select scv file";
        #endregion


        #region MonoBehaviour Callbacks
        private void Awake()
        {
            //最初パネルは消しておく
            _playPanel.SetActive(false);
            _RecordingPanel.SetActive(false);

            //ボタンの設定
            _backButton.interactable = false;
            _playButton.interactable = false;
            _pauseButton.interactable = false;

            //プレイモードが変わった時の挙動
            _playMode.Subscribe(mode => 
            {
                _modeText.text = mode.ToString() + " Mode";

                if(mode == PlayMode.Shadow)
                {
                    _csvPlayer = _shadowReaperPlayer;
                    LiftAndCutterButton(true);
                }
                else
                {
                    _csvPlayer = _robotReaperPlayer;
                    LiftAndCutterButton(false);
                }
            }).AddTo(this);

            _shadowReaperPlayer.EndCSVEvent += OnEndCSVEvent;
            _robotReaperPlayer.EndCSVEvent += OnEndCSVEvent;
        }

        private void Update()
        {
            if(_csvPlayer != null)
            {
                _timerText.text = FloatTimeToString(_csvPlayer.PlayTime);
            }
        }

        private void OnDestroy()
        {
            _shadowReaperPlayer.EndCSVEvent -= OnEndCSVEvent;
            _robotReaperPlayer.EndCSVEvent -= OnEndCSVEvent;
        }
        #endregion

        #region Public method for Button
        public void SelectCSVFile()
        {
            var path = EditorUtility.OpenFilePanel("CSV ファイルの選択", defaultFileDirectory, "csv");

            if(path != "")
            {
                //csvDataの取得
                _csvPlayer.SetCSVData(path);
                _csvPlayer.SetUp();

                //UIの設定
                _fileNameText.text = Path.GetFileName(path);
                _playButton.interactable = true;
                _backButton.interactable = false;
                _pauseButton.interactable = false;

                //フラグの管理
                _isPlayerActive = true;
            }
            else
            {
                Debug.Log("パスが指定されませんでした");
            }
        }

        public void PlayButtonClick()
        {
            if (!_isPlayerActive) return;

            _backButton.interactable = false;
            _playButton.interactable = false;
            _pauseButton.interactable = true;

            _csvPlayer.Play();
        }

        public void PauseButtonClick()
        {
            if (!_isPlayerActive) return;

            _backButton.interactable = true;
            _playButton.interactable = true;
            _pauseButton.interactable = false;

            _csvPlayer.Pause();
        }

        public void StopButtonClick()
        {
            if (_isPlayerActive)
            {
                _csvPlayer.Stop();
            }

            _backButton.interactable = false;
            _playButton.interactable = false;
            _pauseButton.interactable = false;

            LiftAndCutterButton(true);

            _playPanel.SetActive(false);
            
            _fileNameText.text = defaultFileNameText;
            _isPlayerActive = false;
        }

        public void BakcButtonClick()
        {
            if (!_isPlayerActive) return;

            _backButton.interactable = false;
            _playButton.interactable = true;
            _pauseButton.interactable = false;

            _csvPlayer.Back();
        }
        public void SelectRobotModeClick()
        {
            if(_playMode.Value == PlayMode.Robot)
            {
                if (_playPanel.activeSelf)
                {
                    StopButtonClick();
                }
                else
                {
                    _playMode.SetValueAndForceNotify(PlayMode.Robot);
                    _playPanel.SetActive(true);
                }
            }
            else
            {
                StopButtonClick();
                _playMode.Value = PlayMode.Robot;
                _playPanel.SetActive(true);
            }
        }
        public void SelectShadowModeClick()
        {
            if (_playMode.Value == PlayMode.Shadow)
            {
                if (_playPanel.activeSelf)
                {
                    StopButtonClick();
                }
                else
                {
                    _playMode.SetValueAndForceNotify(PlayMode.Shadow);
                    _playPanel.SetActive(true);
                }
            }
            else
            {
                StopButtonClick();
                _playMode.Value = PlayMode.Shadow;
                _playPanel.SetActive(true);
            }
        }

        public void SelectRecordingModeClick()
        {
            _RecordingPanel.SetActive(!_RecordingPanel.activeSelf);
        }

        #endregion

        #region Private method
        private void OnEndCSVEvent()
        {
            _backButton.interactable = true;
            _playButton.interactable = false;
            _pauseButton.interactable = false;
        }

        private void LiftAndCutterButton(bool interactable)
        {
            _downLiftButton.interactable = interactable;
            _upLiftButton.interactable = interactable;
            _stopCutterButton.interactable = interactable;
            _rotateCutterButton.interactable = interactable;
        }

        private string FloatTimeToString(float seconds)
        {
            var hour = Mathf.FloorToInt(seconds / 3600f);
            var min = Mathf.FloorToInt((seconds - hour * 3600f) / 60f);
            var sec = Mathf.FloorToInt((seconds - hour * 3600f - min * 60f));

            //00:00:00 の形式で返す
            return $"{hour:D2}:{min:D2}:{sec:D2}";
        }
        #endregion
    }

}
