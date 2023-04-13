using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace smart3tene.Reaper
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PassCheckPointPanel : MonoBehaviour
    {
        private CanvasGroup _passPanelCanvasGroup;
        private CancellationTokenSource _cancellationTokenSource = new();

        private void Awake()
        {
            _passPanelCanvasGroup = GetComponent<CanvasGroup>();
            _passPanelCanvasGroup.alpha = 0;

            //ReaperEventManager.CheckPointPassEvent += ShowPanel;
        }

        private void OnDisable()
        {
            //ReaperEventManager.CheckPointPassEvent -= ShowPanel;

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
            _passPanelCanvasGroup.alpha = 1;

            await UniTask.Delay(TimeSpan.FromSeconds(1), true, PlayerLoopTiming.Update, ct);

            _passPanelCanvasGroup.alpha = 0;
        }
    }

}
