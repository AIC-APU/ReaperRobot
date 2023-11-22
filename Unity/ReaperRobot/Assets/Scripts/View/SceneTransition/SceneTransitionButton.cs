using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Plusplus.ReaperRobot.Scripts.View.SceneTransition
{
    public class SceneTransitionButton : MonoBehaviour
    {
        #region Private Fields
        [SerializeField] private Button _button;
        [SerializeField] private string _sceneName;
        #endregion

        #region MonoBehaviour Callbacks
        void Start()
        {
            _button.onClick.AddListener(OnClick);
        }

        void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClick);   
        }
        #endregion

        #region Private method
        private void OnClick()
        {
            SceneManager.LoadScene(_sceneName);
        }
        #endregion
    }
}
