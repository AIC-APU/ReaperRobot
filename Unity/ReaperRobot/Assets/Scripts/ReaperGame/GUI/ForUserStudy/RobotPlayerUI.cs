using Cysharp.Threading.Tasks;
using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace smart3tene.Reaper
{
    public class RobotPlayerUI : MonoBehaviour
    {
        #region Serialized Private Fields
        [Header("CSV Player")]
        [SerializeField] private RobotReaperPlayer _robotReaperPlayer;

        [Header("UIs")]
        [SerializeField] private GameObject _RobotPlayerPanel;
        [SerializeField] private TMP_Text _fileNameText;
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _stopButton;
        #endregion

        #region Readonly Field
        readonly string defaultFileDirectory = Path.GetFullPath(Application.streamingAssetsPath + "/../../../InputLog");
        readonly string defaultFileNameText = "select csv file";
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            _RobotPlayerPanel.SetActive(false);
            _fileNameText.text = defaultFileNameText;

            _backButton.interactable = false;
            _playButton.interactable = false;
            _pauseButton.interactable = false;
            _stopButton.interactable = false;

            _robotReaperPlayer.EndCSVEvent += OnEndCSVEvent;
        }

        void Update()
        {
            //_timeText.text = GameTimer.ConvertSecondsToString(_robotReaperPlayer.PlayTime, false);
        }

        private void OnDestroy()
        {
            _robotReaperPlayer.EndCSVEvent -= OnEndCSVEvent;
        }
        #endregion

        #region Public method
        public async void OnClickSelectFile()
        {
            if (_robotReaperPlayer.IsPlaying.Value) _robotReaperPlayer.Pause();

            //初期位置に配置
            _robotReaperPlayer.Reposition();

            await UniTask.Delay(TimeSpan.FromSeconds(0.5));

            var path = OpenDialogUtility.OpenCSVFile("select csv file", defaultFileDirectory);

            if (path != "")
            {
                //ボタンの設定
                _backButton.interactable = false;
                _playButton.interactable = true;
                _pauseButton.interactable = false;
                _stopButton.interactable = true;

                //csvDataの取得
                _robotReaperPlayer.SetUp(path);

                //FileNameTextの設定
                _fileNameText.text = Path.GetFileName(path);
            }
            else
            {
                Debug.LogWarning("パスが指定されませんでした");
            }
        }

        public void OnClickPlay()
        {
            //ボタンの設定
            _backButton.interactable = false;
            _playButton.interactable = false;
            _pauseButton.interactable = true;
            _stopButton.interactable = true;

            //プレイヤーの設定
            _robotReaperPlayer.Play();
        }

        public void OnClickBack()
        {
            //ボタンの設定
            _backButton.interactable = false;
            _playButton.interactable = true;
            _pauseButton.interactable = false;
            _stopButton.interactable = true;

            //プレイヤーの設定
            _robotReaperPlayer.Back();
        }

        public void OnClickPause()
        {
            //ボタンの設定
            _backButton.interactable = true;
            _playButton.interactable = true;
            _pauseButton.interactable = false;
            _stopButton.interactable = true;

            //プレイヤーの設定
            _robotReaperPlayer.Pause();
        }

        public void OnClickStop()
        {
            //ボタンの設定
            _backButton.interactable = false;
            _playButton.interactable = false;
            _pauseButton.interactable = false;
            _stopButton.interactable = false;

            //プレイヤーの設定
            _robotReaperPlayer.Stop();

            //パネルの初期化
            _RobotPlayerPanel.SetActive(false);
            _fileNameText.text = defaultFileNameText;
        }

        public void OnClickSelectRobotMode()
        {
            if (_RobotPlayerPanel.activeSelf)
            {
                OnClickStop();
            }
            else
            {
                _RobotPlayerPanel.SetActive(true);
            }
        }
        #endregion

        #region Private method
        private void OnEndCSVEvent()
        {
            _backButton.interactable = true;
            _playButton.interactable = false;
            _pauseButton.interactable = false;
            _stopButton.interactable = true;
        }
        #endregion
    }

}