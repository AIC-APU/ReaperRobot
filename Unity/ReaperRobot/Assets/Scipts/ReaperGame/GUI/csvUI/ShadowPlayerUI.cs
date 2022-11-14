using Cysharp.Threading.Tasks;
using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace smart3tene.Reaper
{
    public class ShadowPlayerUI : MonoBehaviour
    {
        #region Public Fields
        public bool ControllableRobot { get; private set; } = true;
        #endregion

        #region Serialized Private Fields
        [Header("CSV Player")]
        [SerializeField] private ShadowReaperPlayer _shadowPlayer;

        [Header("UIs")]
        [SerializeField] private GameObject _shadowPlayerPanel;
        [SerializeField] private TMP_Text _fileNameText;
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _rewindButton;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _fastforwardButton;
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
            _shadowPlayerPanel.SetActive(false);
            _fileNameText.text = defaultFileNameText;

            _backButton.interactable = false;
            _rewindButton.interactable = false;
            _playButton.interactable = false;
            _fastforwardButton.interactable = false;
            _pauseButton.interactable = false;
            _stopButton.interactable = false;

            _shadowPlayer.EndCSVEvent += OnEndCSVEvent;
        }

        private void Update()
        {
            var time = _shadowPlayer.PlayTime;
            _timeText.text = GameTimer.ConvertSecondsToString(time, false);
        }

        private void OnDestroy()
        {
            _shadowPlayer.EndCSVEvent -= OnEndCSVEvent;
        }
        #endregion

        #region Public method
        public async void OnClickSelectFile()
        {
            if (_shadowPlayer.IsPlaying.Value) _shadowPlayer.Pause();

            //ロボットのポジションリセット
            _shadowPlayer.RepositionRobot();

            //コントローラ操作を禁止
            ControllableRobot = false;

            await UniTask.Delay(TimeSpan.FromSeconds(0.5));

            var path = OpenDialogUtility.OpenCSVFile("select csv file", defaultFileDirectory);

            if (path != "")
            {
                //csvDataの取得
                _shadowPlayer.SetUp(path);

                //FileNameTextの設定
                _fileNameText.text = Path.GetFileName(path);

                //ボタンの設定
                _backButton.interactable = false;
                _rewindButton.interactable = false;
                _playButton.interactable = true;
                _fastforwardButton.interactable = false;
                _pauseButton.interactable = false;
                _stopButton.interactable = true;

                //ロボット操作を許可
                ControllableRobot = true;
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
            _rewindButton.interactable = true;
            _playButton.interactable = false;
            _fastforwardButton.interactable = true;
            _pauseButton.interactable = true;
            _stopButton.interactable = true;

            //プレイヤーの設定
            _shadowPlayer.Play();
        }

        public void OnClickBack()
        {
            //ボタンの設定
            _backButton.interactable = false;
            _rewindButton.interactable = false;
            _playButton.interactable = true;
            _fastforwardButton.interactable = false;
            _pauseButton.interactable = false;
            _stopButton.interactable = true;

            //プレイヤーの設定
            _shadowPlayer.Back();
        }
        public void OnClickPause()
        {
            //ボタンの設定
            _backButton.interactable = true;
            _rewindButton.interactable = false;
            _playButton.interactable = true;
            _fastforwardButton.interactable = false;
            _pauseButton.interactable = false;
            _stopButton.interactable = true;

            //プレイヤーの設定
            _shadowPlayer.Pause();
        }
        public void OnClickStop()
        {
            //ボタンの設定
            _backButton.interactable = false;
            _rewindButton.interactable = false;
            _playButton.interactable = false;
            _fastforwardButton.interactable = false;
            _pauseButton.interactable = false;
            _stopButton.interactable = false;

            //プレイヤーの設定
            _shadowPlayer.Stop();

            //パネルの初期化
            _shadowPlayerPanel.SetActive(false);
            _fileNameText.text = defaultFileNameText;

            //コントローラ操作を許可
            ControllableRobot = true;
        }
        public void FastForwardButtonDown()
        {
            _shadowPlayer.FastForward(true);
        }

        public void FastForwardButtonUp()
        {
            _shadowPlayer.FastForward(false);
        }

        public void RewindButtonDown()
        {
            _shadowPlayer.Rewind(true);
        }

        public void RewindButtonUp()
        {
            _shadowPlayer.Rewind(false);
        }

        public void OnClickSelectShadowMode()
        {
            if (_shadowPlayerPanel.activeSelf)
            {
                OnClickStop();               
            }
            else
            {
                //パネル表示
                _shadowPlayerPanel.SetActive(true);
            }
        }

        #endregion

        #region Private method
        private void OnEndCSVEvent()
        {
            _backButton.interactable = true;
            _rewindButton.interactable = false;
            _playButton.interactable = false;
            _fastforwardButton.interactable = false;
            _pauseButton.interactable = false;
            _stopButton.interactable = true;
        }
        #endregion
    }
}