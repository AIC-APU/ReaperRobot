using UnityEngine;
using UnityEngine.UI;

namespace Plusplus.ReaperRobot.Scripts.View.GUI
{
    public class EndGameButton : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private Button _button;
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
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        #endregion
    }
}
