using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace smart3tene.Reaper
{
    public class VRSetUp : MonoBehaviour
    {
        #region Private Fields
        private ManualXRControl _manualXRControl;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            //VRモードならVRモードを起動
            if (ViewMode.NowViewMode.Value == ViewMode.ViewModeCategory.REAPER_VR)
            {
                _manualXRControl = new ManualXRControl();
                StartCoroutine(_manualXRControl.StartXRCoroutine());
            }
        }

        private void OnDisable()
        {
            //VRモードの停止
            if (_manualXRControl != null && _manualXRControl.IsRunning)
            {
                var manualXRControl = new ManualXRControl();
                manualXRControl.StopXR();
            }
        }
        #endregion
    }

}
