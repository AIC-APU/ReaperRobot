using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Plusplus.ReaperRobot.Scripts.Data;

namespace Plusplus.ReaperRobot.Scripts.View.Replay
{
    public abstract class BaseReplay : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField] protected ReplayTimer _timer;
        #endregion

        #region Protected fields
        protected List<DataSet> _dataSets = new();
        #endregion

        #region Abstract Method
        public abstract void InitializeReplay(string filePath);
        public abstract void FinalizeReplay();
        protected abstract void Replay(List<DataSet> data, float seconds);
        #endregion

        #region Protected Method
        protected static List<DataSet> GetDataSets(string filePath)
        {
            var csvdata = CSVUtility.Read(filePath);
            var list = new List<DataSet>();

            //一行目はラベルなので飛ばす
            for(int i = 1; i < csvdata.Count; i++)
            {
                list.Add(new DataSet(csvdata[i]));
            }

            return list;
        }

        protected static float ExtractInputH(List<DataSet> data, float seconds)
        {
            var index = FindIndex(data, seconds);
            return data[index].InputHorizontal;
        }

        protected static float ExtractInputV(List<DataSet> data, float seconds)
        {
            var index = FindIndex(data, seconds);
            return data[index].InputVertical;
        }

        protected static bool ExtractLift(List<DataSet> data, float seconds)
        {
            var index = FindIndex(data, seconds);
            return data[index].Lift;
        }

        protected static bool ExtractCutter(List<DataSet> data, float seconds)
        {
            var index = FindIndex(data, seconds);
            return data[index].Cutter;
        }

        protected static Vector3 ExtractPosition(List<DataSet> data, float seconds)
        {
            var index = FindIndex(data, seconds);

            //indexが最大なら、最後のポジションを返す
            if (index == data.Count - 1)
            {
                return data[index].Position;
            }
            else
            {
                //index, index+1 の値を使って線形補完
                var lowPos = data[index].Position;
                var highPos = data[index + 1].Position;

                var lowSec = data[index].Seconds;
                var highSec = data[index + 1].Seconds;

                var rate = (seconds - lowSec) / (highSec - lowSec);

                return Vector3.Lerp(lowPos, highPos, rate);
            }
        }

        protected static float ExtractAngleY(List<DataSet> data, float seconds)
        {
            var index = FindIndex(data, seconds);

            //indexが最大なら、最後の角度を返す
            if (index == data.Count - 1)
            {
                return data[index].AngleY;
            }
            else
            {
                //index, index+1 の値を使って線形補完
                var lowRot = Quaternion.Euler(0, data[index].AngleY, 0);
                var highRot = Quaternion.Euler(0, data[index + 1].AngleY, 0);

                var lowSec = data[index].Seconds;
                var highSec = data[index + 1].Seconds;

                var rate = (seconds - lowSec) / (highSec - lowSec);

                var lerpQuaternion = Quaternion.Lerp(lowRot, highRot, rate);

                return lerpQuaternion.eulerAngles.y;
            }
        }


        /// <summary>
        /// secondsの秒数を超えない最大のindexを返す
        /// </summary>
        /// <param name="dataSets"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        private static int FindIndex(List<DataSet> dataSets, float seconds)
        {
            //秒数が大きすぎる場合、最大のindexを返す
            if (seconds >= dataSets[dataSets.Count - 1].Seconds) return dataSets.Count - 1;

            //秒数が小さすぎる場合、最小のindexを返す
            //インデックス0はヘッダーなので、インデックス1を返す
            if (seconds == 0 || seconds <= dataSets[1].Seconds) return 1;

            //上記以外の場合、secondsの秒数を超えない最大のindexを返す
            int min = 1;
            int max = dataSets.Count - 1;

            //二分探索アルゴリズム
            while (max - min > 1)
            {
                int mid = (max + min) / 2;
                if (seconds < dataSets[mid].Seconds)
                    max = mid;
                else if (seconds > dataSets[mid].Seconds)
                    min = mid;
                else if (seconds == dataSets[mid].Seconds)
                    return mid;
            }
            return min;
        }
        #endregion
    }
}
