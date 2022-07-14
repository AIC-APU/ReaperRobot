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
        #region private Fields
        private bool isConnectToMasterServer = false;
        #endregion

        #region public method
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
                PhotonNetwork.AutomaticallySyncScene = true;
                PhotonNetwork.GameVersion = GameData.GameVersion;
                PhotonNetwork.NickName = GameData.PlayerName;
                PhotonNetwork.OfflineMode = false;
                isConnectToMasterServer = PhotonNetwork.ConnectUsingSettings(); // -> call "OnConnectedToMaster" or "OnDisconnected"     
            }
            else
            {
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public void LeaveRoom()
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.Disconnect();
            }
        }

        public void EndGame()
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("StartMenuScene");
            PhotonNetwork.Disconnect();
        }

        public void CloseApp()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
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
            if (cause != DisconnectCause.DisconnectByClientLogic)
            {
                Debug.LogWarning(cause);
            }
        }

        public override void OnJoinedRoom()
        {
            GameData.PlayerId = PhotonNetwork.LocalPlayer.ActorNumber;

            GameData.CountOfPlayersInRooms.Value = PhotonNetwork.CurrentRoom.PlayerCount;

            if (PhotonNetwork.OfflineMode)
            {
                PhotonNetwork.LoadLevel($"{GameData.NowGameCourse}_{GameData.NowGameMode}");
            }
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = GameData.MaxPlayers }); //-> call OnJoinedRoom
        }

        public override async void OnPlayerEnteredRoom(Player newPlayer)
        {
            GameData.CountOfPlayersInRooms.Value = PhotonNetwork.CurrentRoom.PlayerCount;

            if (PhotonNetwork.IsMasterClient)
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount == GameData.MaxPlayers)
                {
                    PhotonNetwork.CurrentRoom.IsOpen = false;

                    //数秒待ってシーン遷移
                    await UniTask.Delay(TimeSpan.FromSeconds(3));
                    PhotonNetwork.LoadLevel($"{GameData.NowGameCourse}_{GameData.NowGameMode}");
                }
            }
        }

        public override void OnLeftRoom()
        {
            GameData.CountOfPlayersInRooms.Value = 0;
        }
        #endregion
    }
}

