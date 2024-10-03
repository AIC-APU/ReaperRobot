using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plusplus.ReaperRobot.Scripts.View.Web
{
    public class QueryKeySceneTransition : MonoBehaviour
    {
        public static QueryKeySceneTransition Instance;

        private Dictionary<string, string> _sceneDctionary = new()
        {
            {"Field","Field_MultiView"},
            {"Training","Training_MultiView"},
            {"Slope","Slope_MultiView"},
        };

#if UNITY_WEBGL && !UNITY_EDITOR
        void OnEnable()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                var key = QueryReaderUtility.GetValue("scene");
                if (key == "") return;

                LoadScene(_sceneDctionary, key);
            }
            else
            {
                Destroy(gameObject);
            }
        }
#endif

        void LoadScene(Dictionary<string, string> sceneDictionary, string key)
        {
            var sceneName = sceneDictionary.FirstOrDefault(x => x.Key == key).Value;

            if (sceneName != null)
            {
                SceneManager.LoadScene(sceneName);
            }
        }
    }
}
