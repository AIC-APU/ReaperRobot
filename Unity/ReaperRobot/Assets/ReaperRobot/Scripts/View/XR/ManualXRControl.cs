using System.Collections;
using UnityEngine;
using UnityEngine.XR.Management;

namespace Plusplus.ReaperRobot.Scripts.View.XR
{
    //参考: https://docs.unity3d.com/Packages/com.unity.xr.management@4.0/manual/EndUser.html
    internal class ManualXRControl
    {
        internal bool IsRunning => XRGeneralSettings.Instance.InitManagerOnStart;

        internal IEnumerator StartXRCoroutine()
        {
            Debug.Log("Initializing XR...");
            yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

            if (XRGeneralSettings.Instance.Manager.activeLoader == null)
            {
                Debug.LogError("Initializing XR Failed. Check Editor or Player log for details.");
            }
            else
            {
                Debug.Log("Starting XR...");
                XRGeneralSettings.Instance.Manager.StartSubsystems();
            }
        }

        internal void StopXR()
        {
            Debug.Log("Stopping XR...");

            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            Debug.Log("XR stopped completely.");
        }
    }

}