using System;
using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.View.Replay
{
    public class DataSet
    {
        public float Seconds { get; }

        public float InputHorizontal { get; }
        public float InputVertical { get; }

        public bool Lift { get; }
        public bool Cutter { get; }

        public float PositionX { get; }
        public float PositionY { get; }
        public float PositionZ { get; }
        public Vector3 Position => new Vector3(PositionX, PositionY, PositionZ);

        public float AngleY { get; }

        public static string CSVLabel() => $"Time,Input_H,Input_V,Lift,Cutter,PositionX,PositionY,PositionZ,AngleY";
        

        #region Constructor
        public DataSet(float seconds, float inputHorizontal, float inputVertical, bool lift, bool cutter, float positionX, float positionY, float positionZ, float angleY)
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
        }

        public DataSet(string[] data)
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
        }

        public string CSVLine()
        {
            //データをstringに変換するルール
            var time = TimeConverter.ToString(Seconds);
            var inputH = InputHorizontal.ToString();
            var inputV = InputVertical.ToString();
            var lift = Lift ? "1" : "0";
            var cutter = Cutter ? "1" : "0";
            var posX = RoundF(PositionX, 8);
            var posY = RoundF(PositionY, 8);
            var posZ = RoundF(PositionZ, 8);
            var angleY = RoundF(NormalizeAngle(AngleY), 8);

            return $"{time},{inputH},{inputV},{lift},{cutter},{posX},{posY},{posZ},{angleY}";
        }
        #endregion

        #region Private Method
        /// <summary>
        /// 指定した小数点以下桁数で四捨五入
        /// </summary>
        /// <param name="num">四捨五入したい数</param>
        /// <param name="digits">小数点以下の桁数</param>
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
