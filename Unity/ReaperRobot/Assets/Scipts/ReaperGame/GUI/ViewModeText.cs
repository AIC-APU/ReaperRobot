using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;

namespace smart3tene.Reaper
{
    public class ViewModeText : MonoBehaviour
    {
        [SerializeField] TMP_Text _viewModeText;

        private void Awake()
        {
            ViewMode.NowViewMode.Subscribe(mode => _viewModeText.text = mode.ToString());
        }
    }
}

