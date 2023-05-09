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
        private Vector3 _defaultPosition;
        private Vector3 _defaultAngle;
        #endregion

        #region MonoBehaviour Callback
        void Awake()
        {
            _defaultPosition = _reaperTransform.position;
            _defaultAngle = _reaperTransform.eulerAngles;

            _replayManager
                .Time
                .Where(_ => _replayManager.IsDataReady)
                .Subscribe(seconds =>
                {
                    if (seconds != 0)
                    {
                        //通常再生されている時
                        if (_replayManager.IsNomalSpeed) Replay();
                    }
                    else
                    {
                        //初期状態に戻った時
                        _reaperManager.Move(0, 0);
                        ResetPosition(_reaperTransform);
                    }

                })
                .AddTo(this);

            _replayManager
                .IsReplaying
                .Skip(1)
                .Subscribe(x =>
                {
                    if (x)
                    {
                        //停止状態から再生された時
                        ResetPosition(_reaperTransform);
                    }
                    else
                    {
                        //再生状態から停止された時
                        _reaperManager.Move(0, 0);
                    }
                })
                .AddTo(this);
        }
        #endregion

        #region Private method
        protected override void Replay()
        {
            var inputH = _replayManager.GetInputH();
            var inputV = _replayManager.GetInputV();
            _reaperManager.Move(inputH, inputV);
        }

        private void ResetPosition(Transform target)
        {
            var rawPos = _replayManager.GetPosition();
            var offsetPos = _defaultPosition - _replayManager.GetStartPosition();
            var pos = rawPos + offsetPos;

            var rawAngleY = _replayManager.GetAngleY();
            var offsetAngle = _defaultAngle - new Vector3(0, _replayManager.GetStartAngleY(), 0);
            var angleY = rawAngleY + offsetAngle.y;
            var rot = Quaternion.Euler(0, angleY, 0);

            target.transform.SetPositionAndRotation(pos, rot);
        }
        #endregion
    }
}