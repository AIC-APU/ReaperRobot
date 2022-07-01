using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.XR.Management;
using UnityEngine;

namespace smart3tene.Reaper
{
    public class VRGUIManager : MonoBehaviour
    {
        #region public Fields

        #endregion

        #region private Fields

        #endregion

        #region MonoBehaviour Callbacks

        void Awake()
        {
            var manualXRControl = new ManualXRControl();
            StartCoroutine(manualXRControl.StartXRCoroutine());
        }

        void Update()
        {

        }

        #endregion

        #region public method
        public static void SetInitializeXROnStartup(BuildTargetGroup buildTargetGroup, bool enabled)
        {
            var xrGeneralSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(buildTargetGroup);
            xrGeneralSettings.InitManagerOnStart = enabled;
            EditorUtility.SetDirty(xrGeneralSettings);
            AssetDatabase.SaveAssets();
        }
        #endregion

        #region private method

        #endregion
    }
}

