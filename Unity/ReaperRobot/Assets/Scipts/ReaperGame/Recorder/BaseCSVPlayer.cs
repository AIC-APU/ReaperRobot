using System;
using System.Collections.Generic;
using UnityEngine;

namespace smart3tene.Reaper
{
    public abstract class BaseCSVPlayer : MonoBehaviour
    {
        #region Event
        public Action StartPlayEvent;
        public Action PausePlayEvent;
        public Action StopEvent;
        public Action EndCSVEvent;
        #endregion

        #region Public Properties
        public string FilePath { get; set; }
        public float PlayTime { get; protected set; } = 0f;
        #endregion

        #region Protected fields
        protected bool _isPlaying = false;
        protected bool _isFastForward = false;
        protected List<string[]> _csvData = new List<string[]>();
        #endregion

        #region Abstract Method
        public abstract void SetUp();
        public abstract void Pause();
        public abstract void Stop();
        public abstract void Play();
        public abstract void Back();
        public abstract void FastForward(bool isFast); 
        protected abstract void OneFlameMove(List<string[]> data, float seconds);
        #endregion

        //以下のPublicクラスはoverrideせずに使ってほしいです。
        #region Public Method 
        public void SetCSVData(string filePath)
        {
            _csvData.Clear();
            _csvData.AddRange(CSVUtility.Read(filePath));
        }
        #endregion

        //以下のProtectedクラスはoverrideせずに使ってほしいです。
        #region Protected Method
        protected TimeSpan ExtractTimeSpan(List<string[]> data, int index)
        {
            var stringTime = data[index][0];
            var timeArray = stringTime.Split(".");

            var hour = int.Parse(timeArray[0]);
            var min  = int.Parse(timeArray[1]);
            var sec  = int.Parse(timeArray[2]);
            var msec = int.Parse(timeArray[3]) * 10; //小数点以下2桁しかないので、10倍して3桁にしている

            return new TimeSpan(0, hour, min, sec, msec);
        }

        protected float ExtractSeconds(List<string[]> data, int index)
        {
            var timeSpan = ExtractTimeSpan(data, index);
            return (float)timeSpan.TotalSeconds;
        }

        protected Vector2 ExtractInput(List<string[]> data, int index)
        {
            var inputH = float.Parse(data[index][1]);
            var inputV = float.Parse(data[index][2]);

            return new Vector2(inputH, inputV);
        }

        protected Vector2 ExtractInput(List<string[]> data, float seconds)
        {
            var index = FindIndexFromSeconds(data, seconds);
            return ExtractInput(data, index);
        }

        protected Vector3 ExtractPosition(List<string[]> data, int index)
        {
            var posX = float.Parse(data[index][3]);
            var posY = float.Parse(data[index][4]);
            var posZ = float.Parse(data[index][5]);

            return new Vector3(posX, posY, posZ);
        }

        protected Vector3　ExtractPosition(List<string[]> data, float seconds)
        {
            var index = FindIndexFromSeconds(data, seconds);

            //indexが最大なら、最後のポジションを返す
            if (index == data.Count - 1) return ExtractPosition(data, data.Count - 1);

            //index, index+1 の値を使って線形補完
            var lowPos = ExtractPosition(data, index);
            var highPos = ExtractPosition(data, index + 1);

            var lowSec = ExtractSeconds(data, index);
            var highSec = ExtractSeconds(data, index + 1);
            var rate = (seconds - lowSec) / (highSec - lowSec);

            return Vector3.Lerp(lowPos, highPos, rate);
        }

        protected float ExtractAngleY(List<string[]> data, int index)
        {
            return float.Parse(data[index][6]);
        }

        protected float ExtractAngleY(List<string[]> data, float seconds)
        {
            var index = FindIndexFromSeconds(data, seconds);

            //indexが最大なら、最後のポジションを返す
            if (index == data.Count - 1) return ExtractAngleY(data, data.Count - 1);

            //index, index+1 の値を使って線形補完
            var lowAngle = ExtractAngleY(data, index);
            var highAngle = ExtractAngleY(data, index + 1);

            var lowSec = ExtractSeconds(data, index);
            var highSec = ExtractSeconds(data, index + 1);
            var rate = (seconds - lowSec) / (highSec - lowSec);

            return Mathf.Lerp(lowAngle, highAngle, rate);
        }

        protected Quaternion ExtractQuaternion(List<string[]> data, int index)
        {
            return Quaternion.Euler(0, ExtractAngleY(data, index), 0);
        }

        protected Quaternion ExtractQuaternion(List<string[]> data, float seconds)
        {
            return Quaternion.Euler(0, ExtractAngleY(data, seconds), 0);
        }

        protected bool ExtractLift(List<string[]> data, int index)
        {
            var isDown = int.Parse(data[index][7]) == 1;
            return isDown;
        }

        protected bool ExtractLift(List<string[]> data, float seconds)
        {
            var index = FindIndexFromSeconds(data, seconds);
            return ExtractLift(data, index);
        }

        protected bool ExtractCutter(List<string[]> data, int index)
        {
            var isRotate = int.Parse(data[index][8]) == 1;
            return isRotate;
        }

        protected bool ExtractCutter(List<string[]> data, float seconds)
        {
            var index = FindIndexFromSeconds(data, seconds);
            return ExtractCutter(data, index);
        }
        #endregion

        #region Private Method
        /// <summary>
        /// secondsの秒数を超えない最大のindexを返す
        /// </summary>
        /// <param name="data"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        private int FindIndexFromSeconds(List<string[]> data, float seconds)
        {
            //秒数が大きすぎる場合、最大のindexを返す
            if (seconds >= ExtractSeconds(data, data.Count - 1)) return data.Count - 1;

            //秒数が小さすぎる場合、最小のindexを返す
            if (seconds == 0 || seconds <= ExtractSeconds(data, 1)) return 1;

            //上記以外の場合、secondsの秒数を超えない最大のindexを返す
            int min = 1;
            int max = data.Count - 1;

            //二分探索を参考にしたアルゴリズム
            while (max - min > 1)
            {
                int mid = (max + min) / 2;
                if (seconds < ExtractSeconds(data, mid))
                    max = mid;
                else if (seconds > ExtractSeconds(data, mid))
                    min = mid;
                else if (seconds == ExtractSeconds(data, mid))
                    return mid;
            }
            return min;
        }
        #endregion
    }
}