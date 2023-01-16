using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace smart3tene.Reaper
{
    public class NowLoadingPanel : MonoBehaviour
    {
        private void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}