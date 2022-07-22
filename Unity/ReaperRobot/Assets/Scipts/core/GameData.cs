using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace smart3tene
{
    public static class GameData
    {
        #region Const Fields
        public static byte MaxPlayers => MAX_PLAYERS_PER_ROOM;
        const byte MAX_PLAYERS_PER_ROOM = 2;

        public static string GameVersion => GAME_VERSION;
        const string GAME_VERSION = "0.1";
        #endregion


        #region Enum
        public enum GameMode
        {
            SOLO,
            VR,
            MULTI,
        }
        public static GameMode NowGameMode { get; set; } = GameMode.SOLO;

        public enum GameCourse
        {
            SimpleField,
        }
        public static GameCourse NowGameCourse { get; set; } = GameCourse.SimpleField;
        #endregion


        #region Public Property
        public static string PlayerName { get; set; } = "NONAME";
        public static int PlayerId { get; set; } = 1;
        #endregion


        #region ReactiveProperty
        public static ReactiveProperty<int> CountOfPlayersInRooms = new(0);
        #endregion
    }
}
