using System;
using UnityEngine;
using Photon.Pun;

namespace smart3tene
{
    //������p������SingletonMonoBehaviourPunCallbacks�̓V�[����Ɉ�������݂��Ȃ��Ȃ�
    //GameManager�ȂǕ�������ƍ���N���X���p������Ƃ���
    //�Q�l�Fhttps://qiita.com/okuhiiro/items/3d69c602b8538c04a479
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
                    Debug.LogError(t + " ���A�^�b�`���Ă���GameObject�͂���܂���");
                }

                return _instance;
            }
        }

        public static bool Instantiated => _instance != null;

        protected virtual void Awake()
        {
            // ����GameObject�ɃA�^�b�`����Ă��邩���ׂ�.
            // �A�^�b�`����Ă���ꍇ�͔j������.
            if (this != Instance)
            {
                Destroy(this);
                return;
            }

            DontDestroyOnLoad(this.gameObject);
        }
    }
}
