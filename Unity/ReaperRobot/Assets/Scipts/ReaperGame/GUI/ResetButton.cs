using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace smart3tene.Reaper
{
    public class ResetButton : MonoBehaviour
    {
        //ボタン用のメゾット
        public void OnClickResetButton()
        {
            ReaperEventManager.InvokeResetEvent();
        }
    }

}