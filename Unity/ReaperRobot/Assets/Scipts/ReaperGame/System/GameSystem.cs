using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Photon.Pun;

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
        public enum ViewMode
        {
            REAPER,
            TPV,
            FPV,
            VR,
        }
        public ReactiveProperty<ViewMode> NowViewMode { get; private set; } = new ReactiveProperty<ViewMode>(ViewMode.REAPER);        
        #endregion

        #region Serialized private Fields
        public GameObject ReaperInstance => _reaperInstance;
        [SerializeField, Tooltip("�}���`�v���C�̎���null�ɂ��Ă����Ă�������")] private GameObject _reaperInstance = null;

        public GameObject PersonInstance => _personInstance;
        [SerializeField, Tooltip("�}���`�v���C�̎���null�ɂ��Ă����Ă�������")] private GameObject _personInstance = null;

        [SerializeField] private ViewMode _defaultOperationMode = ViewMode.REAPER;

        [SerializeField] private List<Transform> _instantiatePos = new List<Transform>();
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

            NowViewMode.Value = _defaultOperationMode;

            if (!PhotonNetwork.IsConnected)
            {
                //�I�t���C���Ƃ��ĎQ������
                PhotonNetwork.OfflineMode = true;
                PhotonNetwork.JoinRandomRoom();
            }

            var posId = GameData.PlayerId - 1; 
            //������@�̐���
            if (_reaperInstance == null)
            {
                _reaperInstance = PhotonNetwork.Instantiate("ReaperCrawlerResource", _instantiatePos[posId].position, _instantiatePos[posId].rotation, 0);
            }

            //�l���f���̐���
            //VR���[�h�̎��͐l�o���Ȃ��Ă����H
            if(NowViewMode.Value != ViewMode.VR &&�@_personInstance == null)
            {
                var playerBackDistance = 3f;
                _personInstance = PhotonNetwork.Instantiate("PersonModel", _instantiatePos[posId].position + (-1 * _instantiatePos[posId].forward * playerBackDistance), _instantiatePos[posId].rotation, 0);
            }

            //���̑������J�E���g
            _allGrassCount = GameObject.FindGameObjectsWithTag("Grass").Length;

            //�Q�[�����Ԃ̑���
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

            //���Ԃ̃��Z�b�g�͂��邾�낤��

            //�X�R�A�Ƃ����Ă�Ȃ炻������Z�b�g���邩�H
        }

        public void ChangeViewMode()
        {
            switch (NowViewMode.Value)
            {
                case ViewMode.REAPER:
                    NowViewMode.Value = ViewMode.FPV;
                    break;
                case ViewMode.FPV:
                    NowViewMode.Value = ViewMode.TPV;
                    break;
                case ViewMode.TPV:
                    NowViewMode.Value = ViewMode.REAPER;
                    break;
                case ViewMode.VR:
                    //VR���[�h�̎��̓��[�h��ς��Ȃ�
                    break;
                default:
                    break;
            }
        }
        #endregion

    }
}
