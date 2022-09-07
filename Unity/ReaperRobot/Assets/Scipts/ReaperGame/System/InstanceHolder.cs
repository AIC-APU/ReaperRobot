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
                _reaperInstance = PhotonNetwork.Instantiate("ReaperCrawlerResource", _instantiatePos[posId].position, _instantiatePos[posId].rotation, 0);
            }

            //人モデルの生成
            if(_usePerson &&　_personInstance == null)
            {
                var playerBackDistance = 3f;
                _personInstance = PhotonNetwork.Instantiate("RingoResource", _instantiatePos[posId].position + (-1 * _instantiatePos[posId].forward * playerBackDistance), _instantiatePos[posId].rotation, 0);
            }
        }
        #endregion
    }
}
