using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace smart3tene.Reaper
{
    public class ClearPanel : MonoBehaviour
    {
        [SerializeField] GameObject _clearPanel;
        [SerializeField] TMP_Text _clearTimeNum;

        private void Awake()
        {
            ReaperEventManager.AllCheckPointPassEvent += SetClearTime;

            _clearPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            ReaperEventManager.AllCheckPointPassEvent -= SetClearTime;
        }

        private void SetClearTime()
        {
            _clearPanel.SetActive(true);
            _clearTimeNum.text = GameTimer.GetCurrentTimeSpan.ToString(@"hh\:mm\:ss");
        }
    }
}

