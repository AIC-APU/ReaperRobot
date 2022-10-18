using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

namespace smart3tene.Reaper
{
    public class CourseSelectPanel : MonoBehaviour
    {
        #region Private Fields
        [Header("Panel")]
        [SerializeField] private GameObject _courceSelectPanel;
        [SerializeField] private GameObject _titlePanel;
        [SerializeField] private GameObject _nowLoadingPanel;

        [Header("Button")]
        [SerializeField] private GameObject _simpleFieldButton;
        [SerializeField] private GameObject _backButton;
        #endregion

        #region MonoBehaviour Callbacks
        private void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(_simpleFieldButton);

            if (PhotonNetwork.OfflineMode)
            {
                _backButton.SetActive(true);
            }
            else
            {
                _backButton.SetActive(false);
            }
        }
        #endregion

        #region Public method for Button
        public void FieldButtonClick_SimpleField()
        {
            GameData.NowGameCourse = GameData.GameCourse.SimpleField;
            SceneTransitionManager.Instance.RoadScene();

            _courceSelectPanel.SetActive(false);
            _nowLoadingPanel.SetActive(true);
        }

        public void FieldButtonClick_Training()
        {
            GameData.NowGameCourse = GameData.GameCourse.Training;
            SceneTransitionManager.Instance.RoadScene();

            _courceSelectPanel.SetActive(false);
            _nowLoadingPanel.SetActive(true);
        }

        public void FieldButtonClick_UserStudy()
        {
            GameData.NowGameCourse = GameData.GameCourse.UserStudy;
            SceneTransitionManager.Instance.RoadScene();

            _courceSelectPanel.SetActive(false);
            _nowLoadingPanel.SetActive(true);
        }
         
        public void OnClickBackButton()
        {
            SceneTransitionManager.Instance.LeaveAndDisconnect();

            _courceSelectPanel.SetActive(false);
            _titlePanel.SetActive(true);
        }
        #endregion
    }
}