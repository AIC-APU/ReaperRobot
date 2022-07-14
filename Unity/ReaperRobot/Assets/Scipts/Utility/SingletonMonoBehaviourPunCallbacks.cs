using System;
using UnityEngine;
using Photon.Pun;

namespace smart3tene
{
    //これを継承したSingletonMonoBehaviourPunCallbacksはシーン上に一つしか存在しなくなる
    //GameManagerなど複数あると困るクラスが継承するといい
    //参考：https://qiita.com/okuhiiro/items/3d69c602b8538c04a479
    public abstract class SingletonMonoBehaviourPunCallbacks<T> : MonoBehaviourPunCallbacks where T : MonoBehaviourPunCallbacks
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;

                var t = typeof(T);
                _instance = (T)FindObjectOfType(t);
                if (_instance == null)
                {
                    Debug.LogError(t + " をアタッチしているGameObjectはありません");
                }

                return _instance;
            }
        }

        public static bool Instantiated => _instance != null;

        protected virtual void Awake()
        {
            // 他のGameObjectにアタッチされているか調べる.
            // アタッチされている場合は破棄する.
            if (this != Instance)
            {
                Destroy(this);
                return;
            }

            DontDestroyOnLoad(this.gameObject);
        }
    }
}
