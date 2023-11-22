using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Plusplus.ReaperRobot.Scripts.Model;

namespace Plusplus.ReaperRobot.Scripts.View.Replay
{
    public class ReplayManager : MonoBehaviour
    {
        #region Event
        public event Action OnStart;
        public event Action OnStop;
        public event Action OnReset;
        #endregion

        #region Public Fields
        public IReadOnlyReactiveProperty<float> Time => _timer.Time;
        public IReadOnlyReactiveProperty<bool> IsReplaying => _timer.IsRunning;
        public string StringTime => TimeSpan.FromSeconds(_timer.Time.Value).ToString(@"hh\:mm\:ss\:fff");
        public bool IsNomalSpeed => _timer.IsNormalMode;
        public bool IsDataReady => _dataSets.Count != 0;
        public ReaperDataSet NowData => IsDataReady ? ExtractDataSet(_dataSets, _timer.Time.Value) : null;
        #endregion

        #region Private Fields
        private List<ReaperDataSet> _dataSets = new List<ReaperDataSet>();
        private Timer _timer = new Timer();
        private IDisposable _disposable;
        #endregion

        #region Readonly Fields
        [Zenject.Inject] readonly IRoadModel _roadModel;
        #endregion

        #region MonoBehaviour Callbacks
        void OnDestroy()
        {
            _disposable?.Dispose();
        }
        #endregion

        #region Public method
        public async void InitializeData(string filePath)
        {
            //データの初期化
            _dataSets.Clear();

            try
            {
                _dataSets = await _roadModel.Road(filePath);
            }
            catch (System.NullReferenceException e)
            {
                throw new System.NullReferenceException("データが読み込めませんでした。", e);
            }

            //タイマーの初期化
            _timer.StopTimer();
            _timer.ResetTimer();

            //タイムが最大値を超えていたらタイマーストップ
            _disposable?.Dispose();
            _disposable = _timer
                            .Time
                            .Where(time => time >= _dataSets[_dataSets.Count - 1].Seconds)
                            .Subscribe(time =>
                            {
                                _timer.StopTimer();
                                _timer.SetTime(_dataSets[_dataSets.Count - 1].Seconds);
                            });

            OnReset?.Invoke();
        }

        public void StartReplay()
        {
            if (!IsDataReady) throw new System.NullReferenceException("データが読み込まれていません。");

            _timer.StartTimer();
            OnStart?.Invoke();
        }
        public void PauseReplay()
        {
            if (!IsDataReady) throw new System.NullReferenceException("データが読み込まれていません。");

            _timer.StopTimer();
            OnStop?.Invoke();
        }
        public void ResetReplay()
        {
            if (!IsDataReady) throw new System.NullReferenceException("データが読み込まれていません。");

            _timer.StopTimer();
            _timer.ResetTimer();
            OnReset?.Invoke();
        }

        public void SetPlayMode(Timer.PlayMode mode) => _timer.ChangePlayMode(mode);

        public float GetInputH() => ExtractInputH(_dataSets, _timer.Time.Value);
        public float GetInputV() => ExtractInputV(_dataSets, _timer.Time.Value);
        public bool GetLift() => ExtractLift(_dataSets, _timer.Time.Value);
        public bool GetCutter() => ExtractCutter(_dataSets, _timer.Time.Value);
        public Vector3 GetPosition() => ExtractPosition(_dataSets, _timer.Time.Value);
        public Vector3 GetStartPosition() => ExtractPosition(_dataSets, 0f);
        public float GetAngleY() => ExtractAngleY(_dataSets, _timer.Time.Value);
        public float GetStartAngleY() => ExtractAngleY(_dataSets, 0f);
        #endregion

        #region Private method
        private static float ExtractInputH(IReadOnlyList<ReaperDataSet> data, float seconds)
        {
            if (data.Count == 0) throw new System.NullReferenceException();

            var index = FindIndex(data, seconds);
            return data[index].InputHorizontal;
        }

        private static float ExtractInputV(IReadOnlyList<ReaperDataSet> data, float seconds)
        {
            if (data.Count == 0) throw new System.NullReferenceException();

            var index = FindIndex(data, seconds);
            return data[index].InputVertical;
        }

        private static bool ExtractLift(IReadOnlyList<ReaperDataSet> data, float seconds)
        {
            if (data.Count == 0) throw new System.NullReferenceException();

            var index = FindIndex(data, seconds);
            return data[index].Lift;
        }

        private static bool ExtractCutter(IReadOnlyList<ReaperDataSet> data, float seconds)
        {
            if (data.Count == 0) throw new System.NullReferenceException();

            var index = FindIndex(data, seconds);
            return data[index].Cutter;
        }

        private static Vector3 ExtractPosition(IReadOnlyList<ReaperDataSet> data, float seconds)
        {
            if (data.Count == 0) throw new System.NullReferenceException();

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

        private static float ExtractAngleY(IReadOnlyList<ReaperDataSet> data, float seconds)
        {
            if (data.Count == 0) throw new System.NullReferenceException();

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

        private static ReaperDataSet ExtractDataSet(IReadOnlyList<ReaperDataSet> data, float seconds)
        {
            if (data.Count == 0) throw new System.NullReferenceException();

            var index = FindIndex(data, seconds);
            return data[index];
        }

        /// <summary>
        /// secondsの秒数を超えない最大のindexを返す
        /// </summary>
        private static int FindIndex(IReadOnlyList<ReaperDataSet> dataSets, float seconds)
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
