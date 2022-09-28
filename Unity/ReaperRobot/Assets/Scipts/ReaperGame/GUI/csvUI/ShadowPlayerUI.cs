using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UniRx;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace smart3tene.Reaper
{
    public class ShadowPlayerUI : MonoBehaviour
    {
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
        public void OnClickSelectFile()
        {
            var path = EditorUtility.OpenFilePanel("CSV ファイルの選択", defaultFileDirectory, "csv");

            if (path != "")
            {
                //csvDataの取得
                _shadowPlayer.SetUp(path);

                //ロボットの位置の設定
                ReaperEventManager.InvokeResetEvent();

                //FileNameTextの設定
                _fileNameText.text = Path.GetFileName(path);

                //ボタンの設定
                _backButton.interactable = false;
                _rewindButton.interactable = false;
                _playButton.interactable = true;
                _fastforwardButton.interactable = false;
                _pauseButton.interactable = false;
                _stopButton.interactable = true;
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
                _shadowPlayerPanel.SetActive(true);

                //ロボットの位置の設定
                ReaperEventManager.InvokeResetEvent();
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