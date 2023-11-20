using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plusplus.ReaperRobot.Scripts.View.URL
{
    public class URLSceneTransition : MonoBehaviour
    {
        public static URLSceneTransition Instance;

        [Serializable]
        private struct SceneAndKey
        {
            public string key;
            public string sceneName;
        }

        [SerializeField] private List<SceneAndKey> _sceneAndKeys = new();

#if UNITY_WEBGL && !UNITY_EDITOR

        void OnEnable()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                var key = GetSceneKey();
                if (key == "") return;

                LoadScene(_sceneAndKeys, key);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        

#endif

        private Dictionary<string, string> GetQueryDictionary()
        {
            var uri = new Uri(Application.absoluteURL);
            var queryStr = uri.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped);

            var queries = queryStr
                .Split('&')
                .Select(q => q.Split('='))
                .Where(x => x.Length == 2)
                .ToDictionary(x => x[0], x => x[1]);

            return queries;
        }

        private string GetSceneKey()
        {
            var queries = GetQueryDictionary();

            if (queries.ContainsKey("scene"))
            {
                return queries["scene"];
            }
            else
            {
                return "";
            }
        }

        private void LoadScene(IEnumerable<SceneAndKey> sceneAndKeys, string key)
        {
            var sceneAndKey = sceneAndKeys.FirstOrDefault(x => x.key == key);

            if (sceneAndKey.sceneName != null)
            {
                SceneManager.LoadScene(sceneAndKey.sceneName);
            }
        }
    }
}
