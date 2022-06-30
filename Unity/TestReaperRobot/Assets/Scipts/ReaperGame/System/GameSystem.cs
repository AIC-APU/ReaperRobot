using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace smart3tene.Reaper
{
    [DefaultExecutionOrder(-2)]
    public class GameSystem : MonoBehaviour
    {
        #region Event
        public event Action ResetEvent;
        #endregion

        #region public Fields
        public static GameSystem Instance = null;
        #endregion

        #region Enum
        public enum OperationMode
        {
            reaper,
            tpv,
            fpv,
        }
        public ReactiveProperty<OperationMode> NowOperationMode { get; private set; } = new ReactiveProperty<OperationMode>(OperationMode.reaper);
        #endregion

        #region Serialized private Fields
        public GameObject ReaperInstance => _reaperInstance;
        [SerializeField, Tooltip("マルチプレイの時はnullにしておいてください")] private GameObject _reaperInstance = null;

        public GameObject PersonInstance => _personInstance;
        [SerializeField, Tooltip("マルチプレイの時はnullにしておいてください")] private GameObject _personInstance = null;
        #endregion

        #region private Fields
        public int AllGrassCount => _allGrassCount;
        private int _allGrassCount;

        public IReadOnlyReactiveProperty<int> CutGrassCount => _cutGrassCount;
        private ReactiveProperty<int> _cutGrassCount = new(0);

        public IReadOnlyReactiveProperty<float> GameTime => _gameTime;
        private ReactiveProperty<float> _gameTime = new(0);        

        private float _gameStartTime;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            //プレイヤーの生成
            if(_reaperInstance == null)
            {
                var reaperPrefab = (GameObject)Resources.Load("ReaperCrawlerResource");
                _reaperInstance = Instantiate(reaperPrefab, new Vector3(0, 0.05f, 0), Quaternion.identity);
            }

            if(_personInstance == null)
            {
                var personPrefab = (GameObject)Resources.Load("PersonModel");
                _personInstance = Instantiate(personPrefab, new Vector3(0, 0.05f, 0), Quaternion.identity);

                //VRModeだとここ人モデルと一緒にCameraRigも生成する？
            }

            //草の総数をカウント
            _allGrassCount = GameObject.FindGameObjectsWithTag("Grass").Length;

            //ゲーム時間の測定
            _gameStartTime = Time.time;
            this.UpdateAsObservable()
                .Subscribe(_ => _gameTime.Value = Time.time - _gameStartTime)
                .AddTo(this);
        }

        #endregion

        #region Public Method
        public void AddCutGrassCount(int count)
        {
            _cutGrassCount.Value += count;
        }

        public void ResetGrasses()
        {
            ResetEvent?.Invoke();

            //時間のリセットはいるだろうか

            //スコアとかつけてるならそれもリセットするか？
        }

        public void ChangeOperationMode()
        {
            if (NowOperationMode.Value == OperationMode.reaper)
            {
                NowOperationMode.Value = OperationMode.fpv;
            }
            else if (NowOperationMode.Value == OperationMode.fpv)
            {
                NowOperationMode.Value = OperationMode.tpv;
            }
            else if (NowOperationMode.Value == OperationMode.tpv)
            {
                NowOperationMode.Value = OperationMode.reaper;
            }
        }
        #endregion

    }
}
