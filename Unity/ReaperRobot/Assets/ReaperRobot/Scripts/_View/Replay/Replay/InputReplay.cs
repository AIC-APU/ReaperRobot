using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Plusplus.ReaperRobot.Scripts.View.ReaperRobot;

namespace Plusplus.ReaperRobot.Scripts.View.Replay
{
    public class InputReplay : BaseReplay
    {
        #region Serialized Private Fields
        [SerializeField] private ReaperManager _reaperManager;
        [SerializeField] private Transform _reaperTransform;
        #endregion

        #region Private Fields
        private Vector3 _offsetPosition;
        private Vector3 _offsetAngle;
        private IDisposable _timeDisposable;
        private IDisposable _runDisposable;
        #endregion

        #region MonoBehaviour Callbacks
        void OnDestroy()
        {
            _timeDisposable?.Dispose();
            _runDisposable?.Dispose();
        }
        #endregion

        #region Public method
        public override void FinalizeReplay()
        {
            _timeDisposable?.Dispose();
            _runDisposable?.Dispose();
            _dataSets.Clear();
        }

        public override void InitializeReplay(string filePath)
        {
            //データの読み込み
            _dataSets.Clear();
            _dataSets.AddRange(GetDataSets(filePath));

            //位置と角度のオフセットを取得
            _offsetPosition = _reaperTransform.transform.position - ExtractPosition(_dataSets, 0f);
            _offsetAngle = _reaperTransform.transform.eulerAngles - new Vector3(0, ExtractAngleY(_dataSets, 0f), 0);

            _timeDisposable =
                _timer
                .Time
                .Subscribe(seconds =>
                {
                    if (seconds != 0)
                    {
                        //通常再生されている時
                        if(_timer.IsNomalSpeed()) Replay(_dataSets, seconds);
                    }
                    else
                    {
                        //初期状態に戻った時
                        _reaperManager.Move(0, 0);
                        ResetPosition(_dataSets, _timer.Time.Value);
                    }
                });

            _runDisposable =
                _timer
                .IsReplaying
                .Skip(1)
                .Subscribe(isReplaying =>
                {
                    if (isReplaying)
                    {
                        //停止状態から再生された時
                        ResetPosition(_dataSets, _timer.Time.Value);
                    }
                    else
                    {
                        //再生状態から停止された時
                        _reaperManager.Move(0, 0);
                    }
                });
        }
        #endregion

        #region Private method
        protected override void Replay(List<DataSet> data, float seconds)
        {
            var inputH = ExtractInputH(data, seconds);
            var inputV = ExtractInputV(data, seconds);
            _reaperManager.Move(inputH, inputV);
        }

        private void ResetPosition(List<DataSet> data, float seconds)
        {
            var rawPos = ExtractPosition(data, seconds);
            var posx = rawPos.x + _offsetPosition.x;
            var posy = rawPos.y + _offsetPosition.y;
            var posz = rawPos.z + _offsetPosition.z;
            var pos = new Vector3(posx, posy, posz);

            var rawAngleY = ExtractAngleY(data, seconds);
            var angleY = rawAngleY + _offsetAngle.y;
            var rot = Quaternion.Euler(0, angleY, 0);

            _reaperTransform.transform.SetPositionAndRotation(pos, rot);
        }
        #endregion
    }
}