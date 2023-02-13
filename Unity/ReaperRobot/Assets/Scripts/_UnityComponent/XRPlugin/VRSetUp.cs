using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReaperRobot.Scripts.UnityComponent.XRPlugin
{
    public class VRSetUp : MonoBehaviour
    {
        #region Private Fields
        private ManualXRControl _manualXRControl;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            //VRモード起動
            _manualXRControl = new ManualXRControl();
            StartCoroutine(_manualXRControl.StartXRCoroutine());
        }

        private void OnDisable()
        {
            _manualXRControl = new ManualXRControl();
            _manualXRControl.StopXR();
        }
        #endregion
    }
}