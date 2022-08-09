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
            ViewMode.NowViewMode.Value = ViewMode.ViewModeCategory.REAPER_VR;

            //VRモードならVRモードを起動
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
