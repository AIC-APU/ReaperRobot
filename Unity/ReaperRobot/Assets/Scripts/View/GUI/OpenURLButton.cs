using UnityEngine;
using UnityEngine.UI;

namespace Plusplus.ReaperRobot.Scripts.View.GUI
{
    public class OpenURLButton : MonoBehaviour
    {
        [SerializeField] private string url;
        [SerializeField] private Button button;

        void Awake()
        {
            button.onClick.AddListener(Onclick);
        }

        void OnDestroy()
        {
            button.onClick.RemoveListener(Onclick);
        }

        private void Onclick()
        {
            Application.OpenURL(url);
        }
    }
}
