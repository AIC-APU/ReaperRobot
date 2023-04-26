using UnityEngine;
using System;
using UniRx;

namespace Plusplus.ReaperRobot.Scripts.View.Replay
{
    public class ReplayTimer : MonoBehaviour
    {
        #region public Properties
        public IReadOnlyReactiveProperty<float> Time => _time;
        public IReadOnlyReactiveProperty<bool> IsReplaying => _isReplaying;
        public PlayMode NowMode => _playMode;
        #endregion

        #region enum
        public enum PlayMode
        {
            Normal,
            Forward,
            Backward,
        }
        #endregion

        #region Serialized Private Fields
        //インスペクタに表示するためだけの変数
        [SerializeField] private float _nowTime = 0;
        #endregion

        #region Private Fields
        private ReactiveProperty<float> _time = new(0);
        private ReactiveProperty<bool> _isReplaying = new(false);
        private PlayMode _playMode = PlayMode.Normal;
        #endregion

        #region MonoBehaviour Callbacks
        void Update()
        {
            _nowTime = _time.Value;

            if(!_isReplaying.Value) return;

            switch (_playMode)
            {
                case PlayMode.Normal:
                    _time.Value += UnityEngine.Time.deltaTime;
                    break;
                case PlayMode.Forward:
                    _time.Value += UnityEngine.Time.deltaTime * 2;
                    break;
                case PlayMode.Backward:
                    _time.Value = Math.Max(0, _time.Value - UnityEngine.Time.deltaTime * 2);
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }
        #endregion

        #region Public method
        public void StartTimer() => _isReplaying.Value = true;
        public void StopTimer() => _isReplaying.Value = false;
        public void ResetTimer() => _time.Value = 0;
        public void SetPlayModeNormal() => _playMode = PlayMode.Normal;
        public void SetPlayModeForward() => _playMode = PlayMode.Forward;
        public void SetPlayModeBackward() => _playMode = PlayMode.Backward;
        public bool IsNomalSpeed() => _playMode == PlayMode.Normal;
        #endregion
    }
}
