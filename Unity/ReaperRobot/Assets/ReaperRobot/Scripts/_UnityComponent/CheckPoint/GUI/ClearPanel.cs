using UnityEngine;
using TMPro;

namespace Plusplus.ReaperRobot.Scripts.UnityComponent.CheckPoint.GUI
{
    public class ClearPanel : MonoBehaviour
    {
        [SerializeField] GameObject _clearPanel;
        [SerializeField] TMP_Text _clearTimeNum;
        private void Awake()
        {
            CheckPointEvents.OnAllCheckPointPass += SetClearTime;

            _clearPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            CheckPointEvents.OnAllCheckPointPass -= SetClearTime;
        }

        private void SetClearTime()
        {
            _clearPanel.SetActive(true);
        }
    }
}
