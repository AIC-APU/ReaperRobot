using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;
using System;
using Photon.Pun;

namespace smart3tene.Reaper
{
    public class TitlePanel : MonoBehaviour
    {
        #region Serialized Private Fields
        [Header("Panel")]
        [SerializeField] private GameObject _titlePanel;
        [SerializeField] private GameObject _courceSelectPanel;
        [SerializeField] private GameObject _waitingMultiPanel;

        [Header("Button")]
        [SerializeField] private GameObject _soloButton;
        [SerializeField] private GameObject _vrButton;
        [SerializeField] private GameObject _multiButton;

        [Header("Mode")]
        [SerializeField] private bool _useMulti = false;
        [SerializeField] private bool _useVR = false;
        #endregion

        #region Private Fields
        private SceneTransitionManager _sceneTransitionManager;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            if (!_useMulti)
            {
                _multiButton.GetComponent<Button>().interactable = false;
            }

            if (!_useVR)
            {
                _vrButton.GetComponent<Button>().interactable = false;
            }
        }

        private void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(_soloButton);
        }

        #endregion

        #region Public method for Button
        public void OnClickSoloButton()
        {
            GameData.NowGameMode = GameData.GameMode.SOLO;
            SceneTransitionManager.Instance.StartOfflineGame();

            _courceSelectPanel.SetActive(true);
            _titlePanel.SetActive(false);
        }

        public void OnClickVRButton()
        {
            GameData.NowGameMode = GameData.GameMode.VR;
            SceneTransitionManager.Instance.StartOfflineGame();

            _courceSelectPanel.SetActive(true);
            _titlePanel.SetActive(false);
        }

        public void OnClickMultiButton()
        {
            GameData.NowGameMode = GameData.GameMode.MULTI;
            SceneTransitionManager.Instance.StartMultiGame();

            _waitingMultiPanel.SetActive(true);
            _titlePanel.SetActive(false);
        }

        public void OnClickSettingButton()
        {
            throw new System.NotImplementedException();
        }

        public void OnClickExitButton()
        {
            SceneTransitionManager.Instance.CloseApp();
        }
        #endregion
    }

}