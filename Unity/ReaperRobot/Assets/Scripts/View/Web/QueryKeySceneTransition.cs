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

                var key = QueryReaderUtility.GetValue("scene");
                if (key == "") return;

                LoadScene(_sceneAndKeys, key);
            }
            else
            {
                Destroy(gameObject);
            }
        }
#endif

        void LoadScene(IEnumerable<SceneAndKey> sceneAndKeys, string key)
        {
            var sceneAndKey = sceneAndKeys.FirstOrDefault(x => x.key == key);

            if (sceneAndKey.sceneName != null)
            {
                SceneManager.LoadScene(sceneAndKey.sceneName);
            }
        }
    }
}
