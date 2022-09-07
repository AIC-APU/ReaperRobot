using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using System.Linq;

namespace smart3tene.Reaper
{
    [RequireComponent(typeof(PhotonView))]
    public class GrassSynchronizer : MonoBehaviourPun
    {
        //マルチプレイの時に刈った草を完全に同期させたくてこのクラスを作成しました
        //ただ、以下の方法では上手くいかなかったです。
        //今後別の方法を試す時のためにここに残しておきます。

        #region Private Fields
        private List<Grass> grasses = new();
        #endregion

        #region MonoBehaviour Callbacks
        void Start()
        {
            //Grassリストを作る
            grasses.AddRange(FindObjectsOfType<Grass>());

            //各GrassのisCutが変わった時に通信

            foreach (Grass grass in grasses)
            {
                grass.IsCut.Subscribe(x =>
                {
                    if (x)
                    {
                        var index = grasses.IndexOf(grass);
                        photonView.RPC(nameof(RPCGrassCut), RpcTarget.All, index);
                    } 
                }).AddTo(this);
            }
        }
        #endregion

        #region RPC Method
        [PunRPC]
        private void RPCGrassCut(int index)
        {
            //指定されたインデックスの草をisCutに
            //※マルチプレイで試してみると、関係無いはずの草が刈られる
            grasses[index].CutThisGrass();
        }
        #endregion
    }

}
