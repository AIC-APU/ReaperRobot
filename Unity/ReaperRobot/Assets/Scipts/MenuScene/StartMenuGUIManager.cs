using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;
using UnityEngine.UI;
using Photon.Pun;

namespace smart3tene
{
    public class StartMenuGUIManager : MonoBehaviour
    {
        #region Serialized Private Fields
        [Header("SceneTransitionManager")]
        [SerializeField] private SceneTransitionManager _sceneTransitionManager;

        [Header("Panels")]
        [SerializeField] private GameObject _tiltePanel;
        [SerializeField] private GameObject _courseselectPanel;
        [SerializeField] private GameObject _nowLoadingPanel;
        [SerializeField] private GameObject _waitingPanel;
        [SerializeField] private GameObject _multiFailedPanel;

        [Header("Text")]
        [SerializeField] private TMP_Text _roomPlayerNum;

        [Header("Button")]
        [SerializeField] private Button _cancelButton;
        #endregion

        #region Private Fields

        #endregion

        #region MonoBehaviour Callbacks

        void Awake()
        {
            _tiltePanel.SetActive(true);
            _courseselectPanel.SetActive(false);
            _waitingPanel.SetActive(false);
            _multiFailedPanel.SetActive(false);

            GameData.CountOfPlayersInRooms.Subscribe(x => _roomPlayerNum.text = $"{x}/{GameData.MaxPlayers}");

            //_sceneTransitionManager.MultiStartEvent += ShowWaitingPanel;
        }

        private void Update()
        {
            _cancelButton.interactable = PhotonNetwork.InRoom;
        }

        private void OnDestroy()
        {
            //_sceneTransitionManager.MultiStartEvent -= ShowWaitingPanel;
        }
        #endregion

        #region public method
        public void SetSoloMode()
        {
            GameData.NowGameMode = GameData.GameMode.SOLO;
        }

        public void SetVRMode()
        {
            GameData.NowGameMode = GameData.GameMode.VR;
        }
        public void SetMultiMode()
        {
            GameData.NowGameMode = GameData.GameMode.MULTI;
        }
        public void SetSimpleField()
        {
            GameData.NowGameCourse = GameData.GameCourse.SimpleField;

            StartGame();
        }
        public void CanselMulti()
        {
            _sceneTransitionManager.LeaveRoom();
        }
        #endregion

        #region private method
        private void StartGame()
        {
            switch (GameData.NowGameMode)
            {
                case GameData.GameMode.SOLO:
                case GameData.GameMode.VR:
                    ShowNowLoadingPanel();
                    SceneTransitionManager.Instance.StartOfflineGame();
                    break;
                case GameData.GameMode.MULTI:
                    ShowWaitingPanel();
                    SceneTransitionManager.Instance.StartMultiGame();
                    break;
                default:
                    break;
            }
        }

        private void ShowWaitingPanel()
        {
            _waitingPanel.SetActive(true);

            _tiltePanel.SetActive(false);
            _courseselectPanel.SetActive(false);
            _nowLoadingPanel.SetActive(false);
            _multiFailedPanel.SetActive(false);
        }

        private void ShowNowLoadingPanel()
        {
            _nowLoadingPanel.SetActive(true);

            _tiltePanel.SetActive(false);
            _courseselectPanel.SetActive(false);
            _waitingPanel.SetActive(false);
            _multiFailedPanel.SetActive(false);
        }
        #endregion
    }

}
