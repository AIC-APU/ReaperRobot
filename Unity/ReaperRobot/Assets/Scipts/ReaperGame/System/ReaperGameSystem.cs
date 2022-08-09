using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

namespace smart3tene.Reaper
{
    [DefaultExecutionOrder(-2)]
    public class ReaperGameSystem : MonoBehaviour
    {
        #region public Fields
        public static ReaperGameSystem Instance = null;
        #endregion

        #region Serialized private Fields
        public GameObject ReaperInstance => _reaperInstance;
        [SerializeField, Tooltip("マルチプレイの時はnullにしておいてください")] private GameObject _reaperInstance = null;

        public GameObject PersonInstance => _personInstance;
        [SerializeField, Tooltip("マルチプレイの時はnullにしておいてください")] private GameObject _personInstance = null;

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
            if (_reaperInstance == null)
            {
                _reaperInstance = PhotonNetwork.Instantiate("ReaperCrawlerResource", _instantiatePos[posId].position, _instantiatePos[posId].rotation, 0);
            }

            //人モデルの生成
            //VRモードの時は人出さなくていい？
            if(ViewMode.NowViewMode.Value != ViewMode.ViewModeCategory.REAPER_VR &&　_personInstance == null)
            {
                var playerBackDistance = 3f;
                _personInstance = PhotonNetwork.Instantiate("RingoResource", _instantiatePos[posId].position + (-1 * _instantiatePos[posId].forward * playerBackDistance), _instantiatePos[posId].rotation, 0);
            }
        }
        #endregion
    }
}
