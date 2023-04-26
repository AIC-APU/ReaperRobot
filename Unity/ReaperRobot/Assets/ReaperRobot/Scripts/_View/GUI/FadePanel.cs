using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace Plusplus.ReaperRobot.Scripts.View.GUI
{
     [RequireComponent(typeof(CanvasGroup))]
    public class FadePanel : MonoBehaviour
    {
        [SerializeField, Range(1, 10)] private int _fadeInTime = 1;
        [SerializeField, Range(1, 10)] private int _popupTime = 3;
        [SerializeField, Range(1, 10)] private int _fadeOutTime = 1;
        [SerializeField] private bool _useRepeat = false;
        [SerializeField, Range(1, 10)] private int _repeatInterval = 10;

        private CanvasGroup _canvasGroup;
        private CancellationTokenSource _cancellationTokenSource = new();

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;
        }

        private void OnDisable()
        {
            _cancellationTokenSource.Cancel();
        }

        public void FadeinPanel()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            _ = ShowPanelTask(_cancellationTokenSource.Token);
        }

        private async UniTask ShowPanelTask(CancellationToken ct = default)
        {
           while (true)
            {
                //最初は非表示
                _canvasGroup.alpha = 0;

                //フェードで表示
                var time = 0f;
                while (_canvasGroup.alpha < 1)
                {
                    time += Time.deltaTime;
                    time = Mathf.Clamp(time, 0, _fadeInTime);
                    _canvasGroup.alpha = time / _fadeInTime;
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }

                //数秒待つ
                await UniTask.Delay(TimeSpan.FromSeconds(_popupTime), false, PlayerLoopTiming.Update, ct);

                //フェードで非表示
                time = _fadeOutTime;
                while (_canvasGroup.alpha > 0)
                {
                    time -= Time.deltaTime;
                    time = Mathf.Clamp(time, 0, _fadeOutTime);
                    _canvasGroup.alpha = time / _fadeOutTime;
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }

                if (!_useRepeat)
                {
                    break;
                }
                else
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(_repeatInterval), false, PlayerLoopTiming.Update, ct);
                }
            }
        }
    }
}
