using Photon.Pun;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace smart3tene
{
    public class StartMenuGUIManager : MonoBehaviour
    {
        #region Serialized Private Fields
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
        [SerializeField] private GameObject _backButton;
        #endregion

        #region Private Fields
        private SceneTransitionManager _sceneTransitionManager;
        #endregion

        #region MonoBehaviour Callbacks

        void Awake()
        {
            _tiltePanel.SetActive(true);
            _courseselectPanel.SetActive(false);
            _waitingPanel.SetActive(false);
            _multiFailedPanel.SetActive(false);

            GameData.CountOfPlayersInRooms
                .Subscribe(x => _roomPlayerNum.text = $"{x}/{GameData.MaxPlayers}")
                .AddTo(this);

            //↓この呼び方にしないとイベントの解除が失敗する
            _sceneTransitionManager = SceneTransitionManager.Instance;
            _sceneTransitionManager.RoomFilledEvent += ShowCourseSelectPanelForMaster;
        }

        
        private void Update()
        {
            _cancelButton.interactable = PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount < GameData.MaxPlayers;
        }

        private void OnDestroy()
        {
            _sceneTransitionManager.RoomFilledEvent -= ShowCourseSelectPanelForMaster;
        }
        #endregion

        #region public method
        public void SoloButtonClick()
        {
            GameData.NowGameMode = GameData.GameMode.SOLO;
            SceneTransitionManager.Instance.StartOfflineGame();
        }

        public void VRButtonClick()
        {
            GameData.NowGameMode = GameData.GameMode.VR;
            SceneTransitionManager.Instance.StartOfflineGame();
        }
        public void MultiButtonClick()
        {
            GameData.NowGameMode = GameData.GameMode.MULTI;
            SceneTransitionManager.Instance.StartMultiGame();
        }
        public void ExitButtonClick()
        {
            SceneTransitionManager.Instance.CloseApp();
        }
        public void FieldButtonClick_SimpleField()
        {
            GameData.NowGameCourse = GameData.GameCourse.SimpleField;

            SceneTransitionManager.Instance.RoadScene();
        }
        public void FieldButtonClick_Training()
        {
            GameData.NowGameCourse = GameData.GameCourse.Training;

            SceneTransitionManager.Instance.RoadScene();
        }
        public void FieldButtonClick_UserStudy()
        {
            GameData.NowGameCourse = GameData.GameCourse.UserStudy;

            SceneTransitionManager.Instance.RoadScene();
        }
        public void CancelButtonClick()
        {
            SceneTransitionManager.Instance.LeaveAndDisconnect();
        }
        public void BackButtonClick()
        {
            SceneTransitionManager.Instance.LeaveAndDisconnect();
        }
        #endregion

        #region private method
        private void ShowCourseSelectPanelForMaster()
        {
            //MasterClientのみがコースを決めることができる
            if (PhotonNetwork.IsMasterClient)
            {
                _courseselectPanel.SetActive(true);

                _tiltePanel.SetActive(false);
                _nowLoadingPanel.SetActive(false);
                _waitingPanel.SetActive(false);
                _multiFailedPanel.SetActive(false);

                if (PhotonNetwork.OfflineMode)
                {
                    _backButton.SetActive(true);
                }
                else
                {
                    _backButton.SetActive(false);
                }
            }
            else
            {
                _nowLoadingPanel.SetActive(true);
                
                _tiltePanel.SetActive(false);
                _courseselectPanel.SetActive(false);
                _waitingPanel.SetActive(false);
                _multiFailedPanel.SetActive(false);
            }
        }
        #endregion
    }

}
