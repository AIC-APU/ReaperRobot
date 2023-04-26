using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plusplus.ReaperRobot.Scripts.View.SceneTransition
{
    public class ResetScene : MonoBehaviour
    {
        public void ResetThisScene()
        {
            // 現在のシーンを取得
            Scene scene = SceneManager.GetActiveScene();
            // 現在のシーンのビルド番号を取得
            int buildIndex = scene.buildIndex;
            // 取得したビルド番号のシーン（現在のシーン）を読み込む
            SceneManager.LoadScene(buildIndex);
        }
    }

}