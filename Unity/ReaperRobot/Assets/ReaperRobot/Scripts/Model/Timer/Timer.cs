using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Plusplus.ReaperRobot.Scripts.Model
{
    public class Timer
    {
        #region public Properties
        public IReadOnlyReactiveProperty<float> Time => _time;
        public IReadOnlyReactiveProperty<bool> IsRunning => _isRunning;
        public bool IsNormalMode => _playMode == PlayMode.Normal;
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

        #region Private Fields
        private ReactiveProperty<float> _time = new(0);
        private ReactiveProperty<bool> _isRunning = new(false);
        private PlayMode _playMode = PlayMode.Normal;
        private CancellationTokenSource _cancellationTokenSource;
        #endregion

        #region Public method
        public void StartTimer()
        {
            _isRunning.Value = true;

            _cancellationTokenSource = new CancellationTokenSource();
            TimerUpdate(_cancellationTokenSource.Token).Forget();
        }

        public void StopTimer()
        {
            _isRunning.Value = false;

            _cancellationTokenSource?.Cancel();
        }

        public void ResetTimer() => _time.Value = 0;
        public void ChangePlayMode(PlayMode mode) => _playMode = mode;
        public void SetTime(float seconds) => _time.Value = seconds;
        #endregion

        #region Private method
        private async UniTask TimerUpdate(CancellationToken ct = default)
        {
            while (_isRunning.Value)
            {
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
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
        }
        #endregion
    }
}
