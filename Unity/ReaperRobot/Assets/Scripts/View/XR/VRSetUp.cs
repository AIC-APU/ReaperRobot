using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.View.XR
{
    public class VRSetUp : MonoBehaviour
    {
        #region Private Fields
        private ManualXRControl _manualXRControl;
        #endregion

        #region MonoBehaviour Callbacks
        private void OnEnable()
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