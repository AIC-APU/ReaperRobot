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
        public event Action StartWaiting;
        #endregion

        #region private Fields
        private bool isConnectToMasterServer = false;
        #endregion


        #region public method
        public void StartOfflineGame()
        {
            //�I�t���C���Ƃ��ĎQ������
            PhotonNetwork.OfflineMode = true;
            PhotonNetwork.JoinRandomRoom();
        }

        public void StartMultiGame()
        {
            if (!PhotonNetwork.IsConnected)
            {
                //Photon�̃Z�b�g�A�b�v���s���A�I�����C���ŎQ������
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

        //photon�ւ̐ڑ����ł��Ȃ��������ɂ�蒼�����߂Ɏg��
        public void ReloadScene()
        {
            var nowScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(nowScene.name);
        }
        #endregion

        #region private method

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

            int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            GameData.CountOfPlayersInRooms.Value = playerCount;

            if (PhotonNetwork.OfflineMode)
            {
                PhotonNetwork.LoadLevel($"{GameData.NowGameCourse}_{GameData.NowGameMode}");
            }
            else if (playerCount != GameData.MaxPlayers)
            {
                StartWaiting.Invoke();
            }
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log(message);
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

                    Debug.Log("�J�ڂ����܂�");

                    await UniTask.Delay(TimeSpan.FromSeconds(3));
                    PhotonNetwork.LoadLevel($"{GameData.NowGameCourse}_{GameData.NowGameMode}");
                }
            }
        }
        #endregion
    }
}

