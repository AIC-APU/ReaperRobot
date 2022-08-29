using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace smart3tene.Reaper
{
    public class ViewModeText : MonoBehaviour
    {
        [SerializeField] LocalizeStringEvent _localizeStringEvent;

        private void Start()
        {
            ViewMode.NowViewMode.Subscribe(mode => 
            {
                _localizeStringEvent.StringReference.TableEntryReference = mode.ToString();

                _localizeStringEvent.RefreshString();
            });
        }
    }
}

