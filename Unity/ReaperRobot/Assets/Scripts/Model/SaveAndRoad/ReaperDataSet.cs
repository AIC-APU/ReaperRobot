using System;
using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.Model
{
    public class ReaperDataSet
    {
        public float Seconds { get; }
        public float InputHorizontal { get; }
        public float InputVertical { get; }
        public bool Lift { get; }
        public bool Cutter { get; }
        public float PositionX { get; }
        public float PositionY { get; }
        public float PositionZ { get; }
        public float AngleY { get; }

        public string[] StringArray { get; }
        public string StringTime => TimeConverter.ToString(Seconds);
        public Vector3 Position => new Vector3(PositionX, PositionY, PositionZ);


        #region Constructor
        public ReaperDataSet(float seconds, float inputHorizontal, float inputVertical, bool lift, bool cutter, float positionX, float positionY, float positionZ, float angleY)
        {
            Seconds = seconds;
            InputHorizontal = inputHorizontal;
            InputVertical = inputVertical;
            Lift = lift;
            Cutter = cutter;
            PositionX = positionX;
            PositionY = positionY;
            PositionZ = positionZ;
            AngleY = angleY;

            StringArray = CreatStringArray();
        }

        public ReaperDataSet(string[] data)
        {
            //stringをデータに変数するルール
            Seconds = TimeConverter.ToSeconds(data[0]);
            InputHorizontal = float.Parse(data[1]);
            InputVertical = float.Parse(data[2]);
            Lift = data[3] == "1";
            Cutter = data[4] == "1";
            PositionX = float.Parse(data[5]);
            PositionY = float.Parse(data[6]);
            PositionZ = float.Parse(data[7]);
            AngleY = float.Parse(data[8]);

            StringArray = CreatStringArray();
        }
        #endregion

        #region Public Method
        public static string[] Label() => new string[]
        {
            "Time",
            "Input_H",
            "Input_V",
            "Lift",
            "Cutter",
            "PositionX",
            "PositionY",
            "PositionZ",
            "AngleY"
        };
        #endregion

        #region Private Method
        private string[] CreatStringArray()
        {
            //データをstringに変換するルール
            var time = StringTime;
            var inputH = InputHorizontal.ToString();
            var inputV = InputVertical.ToString();
            var lift = Lift ? "1" : "0";
            var cutter = Cutter ? "1" : "0";
            var posX = RoundF(PositionX, 8).ToString();
            var posY = RoundF(PositionY, 8).ToString();
            var posZ = RoundF(PositionZ, 8).ToString();
            var angleY = RoundF(NormalizeAngle(AngleY), 8).ToString();

            return new string[] { time, inputH, inputV, lift, cutter, posX, posY, posZ, angleY };
        }

        private float RoundF(float num, int digits)
        {
            var multipile = (float)Math.Pow(10, digits);
            return Mathf.Round(num * multipile) / multipile;
        }

        private float NormalizeAngle(float angle)
        {
            if (angle > 180)
            {
                angle -= 360;
            }
            else if (angle < -180)
            {
                angle += 360;
            }

            return angle;
        }
        #endregion
    }
}
