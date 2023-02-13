using System.Collections;
using UnityEngine;
using UnityEngine.XR.Management;

namespace ReaperRobot.Scripts.UnityComponent.XRPlugin
{
    //参考: https://docs.unity3d.com/Packages/com.unity.xr.management@4.0/manual/EndUser.html
    public class ManualXRControl
    {
        public bool IsRunning => XRGeneralSettings.Instance.InitManagerOnStart;

        public IEnumerator StartXRCoroutine()
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

        public void StopXR()
        {
            Debug.Log("Stopping XR...");

            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            Debug.Log("XR stopped completely.");
        }
    }

}