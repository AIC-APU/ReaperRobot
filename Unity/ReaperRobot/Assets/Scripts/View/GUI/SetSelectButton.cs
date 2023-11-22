using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Plusplus.ReaperRobot.Scripts.View.GUI
{
    public class SetSelectButton : MonoBehaviour
    {
        //ボタンが配置されているパネルやキャンバスなどにアタッチしてください。
        [SerializeField] private Button _selectButton;
        void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(_selectButton.gameObject);
        }

        void OnDisable()
        {
            if (EventSystem.current == null) return;
            
            var selected = EventSystem.current.currentSelectedGameObject;
            if (selected != null && selected == _selectButton.gameObject)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }
}
