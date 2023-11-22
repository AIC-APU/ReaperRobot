using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Plusplus.ReaperRobot.Scripts.View.SceneTransition
{
    public class ResetScene : MonoBehaviour
    {
        [SerializeField] private Button _resetButton;

        #region MonoBehaviour Callbacks
        void Start()
        {
            _resetButton.onClick.AddListener(OnClick);
        }

        void OnDestroy()
        {
            _resetButton.onClick.RemoveListener(OnClick);
        }
        #endregion 

        #region Private method
        private void OnClick()
        {
            // 現在のシーンを取得
            Scene scene = SceneManager.GetActiveScene();
            // 現在のシーンのビルド番号を取得
            int buildIndex = scene.buildIndex;
            // 取得したビルド番号のシーン（現在のシーン）を読み込む
            SceneManager.LoadScene(buildIndex);
        }
        #endregion
    }
}