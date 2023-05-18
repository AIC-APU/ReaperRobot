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

            //再生時の処理
            _replayManager
                .Time
                .Where(_ => _replayManager.IsDataReady)
                .Where(_ => _replayManager.IsNomalSpeed)
                .Where(seconds => seconds != 0)
                .Subscribe(seconds => Replay())
                .AddTo(this);

            //リセット時の処理
            _replayManager.OnReset += ResetPosition;
            _replayManager.OnReset += StopReaper;

            //再生開始時の処理
            _replayManager.OnStart += ResetPosition;

            //停止時の処理
            _replayManager.OnStop += StopReaper;
        }
        void OnDestroy()
        {
            _replayManager.OnReset -= ResetPosition;
            _replayManager.OnReset -= StopReaper;
            _replayManager.OnStart -= ResetPosition;
            _replayManager.OnStop -= StopReaper;
        }
        #endregion

        #region Private method
        protected override void Replay()
        {
            var inputH = _replayManager.GetInputH();
            var inputV = _replayManager.GetInputV();
            _reaperManager.Move(inputH, inputV);
        }

        private void ResetPosition()
        {
            var rawPos = _replayManager.GetPosition();
            var offsetPos = _defaultPosition - _replayManager.GetStartPosition();
            var pos = rawPos + offsetPos;

            var rawAngleY = _replayManager.GetAngleY();
            var offsetAngle = _defaultAngle - new Vector3(0, _replayManager.GetStartAngleY(), 0);
            var angleY = rawAngleY + offsetAngle.y;
            var rot = Quaternion.Euler(0, angleY, 0);

            _reaperTransform.transform.SetPositionAndRotation(pos, rot);
        }

        private void StopReaper()
        {
            _reaperManager.Move(0, 0);
        }
        #endregion
    }
}