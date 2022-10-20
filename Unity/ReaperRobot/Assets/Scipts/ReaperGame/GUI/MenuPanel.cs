using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Photon.Pun;

namespace smart3tene.Reaper
{
    public class MenuPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _menuPanel;
        [SerializeField] private GameObject _menuTopButton;

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            ReaperEventManager.OpenMenuEvent += ShowMenu;
            ReaperEventManager.CloseMenuEvent += HideMenu;

            _menuPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            ReaperEventManager.OpenMenuEvent -= ShowMenu;
            ReaperEventManager.CloseMenuEvent -= HideMenu;
        }
        #endregion

        #region Public Methods for Button
        public void OnClickCloseMenu()
        {
            ReaperEventManager.InvokeCloseMenuEvent();
        }

        public void OnClickEndGame()
        {
            if(SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.EndGame();

                //PUnrpcで全員のゲームを終了させる
            }
            else
            {
                Debug.LogWarning("SceneTransitionManager がありません。");

                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #else
                Application.Quit();
                #endif
            }
        }
        #endregion

        #region Private Methods
        private void ShowMenu()
        {
            if (!_menuPanel.activeSelf)
            {
                _menuPanel.SetActive(true);
                EventSystem.current.SetSelectedGameObject(_menuTopButton);
            }
        }

        private void HideMenu()
        {
            if (_menuPanel.activeSelf)
            {
                _menuPanel.SetActive(false);
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
        #endregion

        #region RPC Methods

        #endregion
    }

}
