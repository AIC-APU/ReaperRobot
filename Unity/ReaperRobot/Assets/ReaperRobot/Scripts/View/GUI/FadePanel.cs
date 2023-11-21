using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace Plusplus.ReaperRobot.Scripts.View.GUI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadePanel : MonoBehaviour
    {
        [SerializeField, Range(1, 10)] private int FadeInTime = 1;
        [SerializeField, Range(1, 10)] private int PopupTime = 3;
        [SerializeField, Range(1, 10)] private int FadeOutTime = 1;
        [SerializeField] private bool UseRepeat = false;
        [SerializeField, Range(1, 10)] private int RepeatInterval = 10;

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

        public void SetRepeat(bool value)
        {
            UseRepeat = value;
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
                    time = Mathf.Clamp(time, 0, FadeInTime);
                    _canvasGroup.alpha = time / FadeInTime;
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }

                //数秒待つ
                await UniTask.Delay(TimeSpan.FromSeconds(PopupTime), false, PlayerLoopTiming.Update, ct);

                //フェードで非表示
                time = FadeOutTime;
                while (_canvasGroup.alpha > 0)
                {
                    time -= Time.deltaTime;
                    time = Mathf.Clamp(time, 0, FadeOutTime);
                    _canvasGroup.alpha = time / FadeOutTime;
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }

                if (!UseRepeat)
                {
                    break;
                }
                else
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(RepeatInterval), false, PlayerLoopTiming.Update, ct);
                }
            }
        }
    }
}
