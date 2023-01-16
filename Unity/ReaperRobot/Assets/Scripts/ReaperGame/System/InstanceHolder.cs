using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

namespace smart3tene.Reaper
{
    [DefaultExecutionOrder(-2)]
    public class InstanceHolder : MonoBehaviour
    {
        #region public Fields
        public static InstanceHolder Instance = null;
        public GameObject ReaperInstance => _reaperInstance;
        public GameObject PersonInstance => _personInstance;
        #endregion

        #region Serialized private Fields
        [Header("Reaper")]
        [SerializeField, Tooltip("マルチプレイの時はnullにしておいてください")] private GameObject _reaperInstance = null;
        [SerializeField] private bool _useReaper = true;

        [Header("Person")]
        [SerializeField, Tooltip("マルチプレイの時はnullにしておいてください")] private GameObject _personInstance = null;
        [SerializeField] private bool _usePerson = true;

        [Header("Instantiate Positions")]
        [SerializeField] private List<Transform> _instantiatePos = new List<Transform>();
        #endregion

        #region Private Fields
        private Vector3 _defaultReaperPos;
        private Quaternion _defaultReaperRot;
        private Vector3 _defaultPersonPos;
        private Quaternion _defaultPersonRot;
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

            if (!PhotonNetwork.IsConnected)
            {
                //オフラインとして参加する
                PhotonNetwork.OfflineMode = true;
                PhotonNetwork.JoinRandomRoom();
            }

            var posId = GameData.PlayerId - 1; 
            //草刈り機の生成
            if (_useReaper && _reaperInstance == null)
            {
                _defaultReaperPos = _instantiatePos[posId].position;
                _defaultReaperRot = _instantiatePos[posId].rotation;

                _reaperInstance = PhotonNetwork.Instantiate("ReaperCrawlerResource", _defaultReaperPos, _defaultReaperRot, 0);
            }
            else if(_useReaper)
            {
                _defaultReaperPos = _reaperInstance.transform.position;
                _defaultReaperRot = _reaperInstance.transform.rotation;
            }

            //人モデルの生成
            if(_usePerson &&　_personInstance == null)
            {
                var playerBackDistance = 3f;

                _defaultPersonPos = _instantiatePos[posId].position + (-1 * _instantiatePos[posId].forward * playerBackDistance);
                _defaultPersonRot = _instantiatePos[posId].rotation;

                _personInstance = PhotonNetwork.Instantiate("RingoResource", _defaultPersonPos, _defaultPersonRot, 0);
            }
            else if(_usePerson)
            {
                _defaultPersonPos = _personInstance.transform.position;
                _defaultPersonRot = _personInstance.transform.rotation;
            }

            ReaperEventManager.ResetEvent += ResetReaperAndPerson;
        }

        private void OnDestroy()
        {
            ReaperEventManager.ResetEvent -= ResetReaperAndPerson;
        }
        #endregion

        #region Private Method
        private void ResetReaperAndPerson()
        {
            if (_useReaper)
            {
                _reaperInstance.transform.position = _defaultReaperPos;
                _reaperInstance.transform.rotation = _defaultReaperRot;
            }

            if (_usePerson)
            {
                _personInstance.transform.position = _defaultPersonPos;
                _personInstance.transform.rotation = _defaultPersonRot;
            }
        }

        #endregion
    }
}
