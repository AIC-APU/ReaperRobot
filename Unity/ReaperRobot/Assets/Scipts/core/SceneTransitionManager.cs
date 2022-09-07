using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Cysharp.Threading.Tasks;
using UniRx;


namespace smart3tene
{
    public class SceneTransitionManager : SingletonMonoBehaviourPunCallbacks<SceneTransitionManager>
    {
        #region Event
        public event Action RoomFilledEvent;
        #endregion

        #region private Fields
        private bool isConnectToMasterServer = false;
        #endregion

        #region MonoBehaviour Callbacks
        private void OnDestroy()
        {
            LeaveAndDisconnect();
        }

        #endregion


        #region public Method
        public void StartOfflineGame()
        {
            //オフラインとして参加する
            PhotonNetwork.OfflineMode = true;
            PhotonNetwork.JoinRandomRoom();
        }

        public void StartMultiGame()
        {
            if (!PhotonNetwork.IsConnected)
            {
                //Photonのセットアップを行い、オンラインで参加する
                PhotonNetwork.OfflineMode = false;
                PhotonNetwork.AutomaticallySyncScene = true;
                PhotonNetwork.GameVersion = GameData.GameVersion;
                PhotonNetwork.NickName = GameData.PlayerName;

                PhotonNetwork.SendRate = 20; // 1秒間にメッセージ送信を行う回数
                PhotonNetwork.SerializationRate = 10; // 1秒間にオブジェクト同期を行う回数

                PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout = 30000; // in milliseconds. any high value for debug

                PhotonNetwork.ServerPortOverrides = PhotonPortDefinition.AlternativeUdpPorts;

                isConnectToMasterServer = PhotonNetwork.ConnectUsingSettings(); // -> call "OnConnectedToMaster" or "OnDisconnected"     
            }
            else
            {
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public void LeaveAndDisconnect()
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }

            if (PhotonNetwork.IsConnected)
            {        
                PhotonNetwork.Disconnect();
            }
        }

        public async void EndGame()
        {
            PhotonNetwork.AutomaticallySyncScene = false;
            
            await SceneManager.LoadSceneAsync("StartMenu", LoadSceneMode.Single);

            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
            }
        }

        public void CloseApp()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        public void RoadScene()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                //GameDataを参照し、ロードするシーンを決めている
                PhotonNetwork.LoadLevel($"{GameData.NowGameCourse}_{GameData.NowGameMode}");
            }    
        }

        //photonへの接続ができなかった時にやり直すために使う
        public void ReloadScene()
        {
            var nowScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(nowScene.name);
        }
        #endregion

        #region Photon Callbacks
        public override void OnConnectedToMaster()
        {
            if (isConnectToMasterServer)
            {
                PhotonNetwork.JoinRandomRoom(); //-> call OnJoinedRoom or OnJoinRandomFailed 
                isConnectToMasterServer = false;
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            switch (cause)
            {
                case DisconnectCause.None:
                    break;
                case DisconnectCause.DisconnectByClientLogic:
                    break;
                default:
                    Debug.LogWarning(cause);
                    break;
            }
        }

        public override void OnJoinedRoom()
        {
            GameData.PlayerId = PhotonNetwork.LocalPlayer.ActorNumber;
            GameData.CountOfPlayersInRooms.Value = PhotonNetwork.CurrentRoom.PlayerCount;

            if (PhotonNetwork.OfflineMode)
            {
                RoomFilledEvent?.Invoke();
            }
            else if(PhotonNetwork.CurrentRoom.PlayerCount == GameData.MaxPlayers)
            {
                RoomFilledEvent?.Invoke();
            }
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = GameData.MaxPlayers }); //-> call OnJoinedRoom
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            GameData.CountOfPlayersInRooms.Value = PhotonNetwork.CurrentRoom.PlayerCount;

            if (PhotonNetwork.CurrentRoom.PlayerCount == GameData.MaxPlayers)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;

                RoomFilledEvent?.Invoke();
            }
        }

        public override void OnLeftRoom()
        {
            GameData.CountOfPlayersInRooms.Value = 0;
        }
        #endregion
    }
}

