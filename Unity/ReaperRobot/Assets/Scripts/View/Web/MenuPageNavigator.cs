using System;
using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.View.Web
{
    public class MenuPageNavigator : MonoBehaviour
    {
        public void GoToMenuPage()
        {
            //#if UNITY_WEBGL && !UNITY_EDITOR
            var key = QueryReaderUtility.GetValue("scene");
            if (key == "") return;

            var url = GetMenuURL();
            Application.ExternalEval($"location.href = '{url}'");
            //#endif
        }

        string GetMenuURL()
        {
            var uri = new Uri(Application.absoluteURL);
            var host = uri.Host;
            return "https://" + host + "/training/agridigitaltwin/";
        }
    }
}
