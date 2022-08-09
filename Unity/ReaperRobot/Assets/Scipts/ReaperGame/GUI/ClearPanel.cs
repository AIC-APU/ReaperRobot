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
            _clearPanel.SetActive(false);
            ReaperEventManager.AllCheckPointPathEvent += SetClearTime;
        }

        private void OnDisable()
        {
            ReaperEventManager.AllCheckPointPathEvent -= SetClearTime;
        }

        private void SetClearTime()
        {
            _clearPanel.SetActive(true);
            _clearTimeNum.text = GameTimer.GetCurrentTimeSpan.ToString(@"hh\:mm\:ss");
        }
    }
}

