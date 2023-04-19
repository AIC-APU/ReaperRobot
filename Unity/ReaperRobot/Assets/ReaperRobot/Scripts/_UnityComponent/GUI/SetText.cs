using UnityEngine;
using TMPro;

namespace Plusplus.ReaperRobot.Scripts.UnityComponent.GUI
{
    public class SetText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        public void SetTextValue(string text)
        {
            _text.text = text;
        }
    }
}
