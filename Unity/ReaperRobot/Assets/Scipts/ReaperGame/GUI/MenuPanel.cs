using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace smart3tene.Reaper
{
    public class MenuPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _menuPanel;
        [SerializeField] private GameObject _menuTopButton;

        private void Awake()
        {
            _menuPanel.SetActive(false);

            ReaperEventManager.MenuEvent += ShowAndHideMenu;
        }

        private void OnDestroy()
        {
            ReaperEventManager.MenuEvent -= ShowAndHideMenu;
        }

        public void ShowAndHideMenu()
        {
            if (_menuPanel.activeSelf)
            {
                //メニューを閉じる時の挙動
                _menuPanel.SetActive(false);

                EventSystem.current.SetSelectedGameObject(null);

                Time.timeScale = 1f;

            }
            else
            {
                //メニューを開く時の挙動
                _menuPanel.SetActive(true);

                EventSystem.current.SetSelectedGameObject(_menuTopButton);

                Time.timeScale = 0f;
            }
        }

        public void EndGameButtonClick()
        {
            if(SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.EndGame();
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
    }

}
