using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace smart3tene.Reaper 
{
    public class MultiFailedPanel : MonoBehaviour
    {
        #region Serialized Private Fields
        [Header("Panel")]
        [SerializeField] private GameObject _multiFailedPanel;
        [SerializeField] private GameObject _titlePanel;
        [SerializeField] private GameObject _courseSelectPanel;
        [SerializeField] private GameObject _waitingMultiPanel;
        [SerializeField] private GameObject _nowLoadingPanel;

        [Header("Button")]
        [SerializeField] private GameObject _restartButton;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            //通信が切れたことを示すeventにShowFailedPanelを登録する

        }

        private void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(_restartButton);
        }

        private void OnDestroy()
        {
            //登録解除
        }
        #endregion

        #region Public method for Button
        public void OnClickRestartButton()
        {
            SceneTransitionManager.Instance.LeaveAndDisconnect();

            _multiFailedPanel.SetActive(false);
            _titlePanel.SetActive(true);
        }
        #endregion

        #region private method
        public void ShowFailedPanel()
        {
            _multiFailedPanel.SetActive(true);

            _titlePanel.SetActive(false);
            _courseSelectPanel.SetActive(false);
            _waitingMultiPanel.SetActive(false);
            _nowLoadingPanel.SetActive(false);
        }
        #endregion
    }

}