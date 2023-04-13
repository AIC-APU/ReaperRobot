using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UniRx;
using Photon.Pun;
using Cysharp.Threading.Tasks;

namespace smart3tene.Reaper
{
    public class WaitingMultiPanel : MonoBehaviour
    {
        #region Serialized Private Fields
        [Header("Panel")]
        [SerializeField] private GameObject _waitingMultiPanel;
        [SerializeField] private GameObject _titlePanel;
        [SerializeField] private GameObject _courseSelectPanel;
        [SerializeField] private GameObject _nowLoadingPanel;

        [Header("Button")]
        [SerializeField] private GameObject _cancelButtonObject;

        [Header("Text")]
        [SerializeField] private TMP_Text _roomPlayerNum;
        #endregion

        #region private Fields
        private SceneTransitionManager _sceneTransitionManager;
        private Button _cancelButton;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        { 
            GameData.CountOfPlayersInRooms
              .Subscribe(x => _roomPlayerNum.text = $"{x}/{GameData.MaxPlayers}")
              .AddTo(this);

            //↓この呼び方にしないとイベントの解除が失敗する
            _sceneTransitionManager = SceneTransitionManager.Instance;
            _sceneTransitionManager.RoomFilledEvent += ShowCourseSelectPanelForMaster;

            _cancelButton = _cancelButtonObject.GetComponent<Button>();
            _cancelButton.interactable = false;
        }

        private async void OnEnable()
        {
            await UniTask.WaitUntil(() => PhotonNetwork.InRoom);

            _cancelButton.interactable = true;

            EventSystem.current.SetSelectedGameObject(_cancelButtonObject);
        }

        private void OnDisable()
        {
            _cancelButton.interactable = false;
        }

        private void OnDestroy()
        {
            _sceneTransitionManager.RoomFilledEvent -= ShowCourseSelectPanelForMaster;
        }
        #endregion

        #region Public method for Button
        public void OnClickCancelButton()
        {
            SceneTransitionManager.Instance.LeaveAndDisconnect();

            _waitingMultiPanel.SetActive(false);
            _titlePanel.SetActive(true);
        }
        #endregion


        #region Private Fields
        private void ShowCourseSelectPanelForMaster()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _courseSelectPanel.SetActive(true);
            }
            else
            {
                _nowLoadingPanel.SetActive(true);
            }

            _waitingMultiPanel.SetActive(false);
        }
        #endregion
    }

}