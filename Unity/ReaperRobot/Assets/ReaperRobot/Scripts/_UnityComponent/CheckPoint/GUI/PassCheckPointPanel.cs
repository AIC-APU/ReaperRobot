using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace Plusplus.ReaperRobot.Scripts.UnityComponent.CheckPoint.GUI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PassCheckPointPanel : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        private CancellationTokenSource _cancellationTokenSource = new();

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;

            CheckPointEvents.OnCheckPointPass += ShowPanel;
        }

        private void OnDisable()
        {
            CheckPointEvents.OnCheckPointPass -= ShowPanel;

            _cancellationTokenSource.Cancel();
        }

        private void ShowPanel()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            _ = ShowPanelTask(_cancellationTokenSource.Token);
        }

        private async UniTask ShowPanelTask(CancellationToken ct = default)
        {
            _canvasGroup.alpha = 1;

            await UniTask.Delay(TimeSpan.FromSeconds(1), true, PlayerLoopTiming.Update, ct);

            _canvasGroup.alpha = 0;
        }
    }
}

