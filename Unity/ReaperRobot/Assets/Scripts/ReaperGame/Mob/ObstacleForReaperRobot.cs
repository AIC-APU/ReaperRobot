using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace smart3tene.Reaper 
{
    public class ObstacleForReaperRobot : MonoBehaviour
    {
        #region MonoBehaviour Callbacks
        private void OnCollisionEnter(Collision collision)
        {
            //衝突してきたものがロボットであることを判別
            //
            // if (collision.gameObject.transform.root.GetComponent<ReaperManager>())
            // {
            //     //マルチプレイ時、衝突したのが自分以外のロボットだったら何もしない
            //     if (PhotonNetwork.IsConnected && !collision.gameObject.transform.root.GetComponent<PhotonView>().IsMine) return;

            //     //ペナルティイベント発生
            //     ReaperEventManager.InvokePenaltyEvent();
            // }
        }
        #endregion
    }
}