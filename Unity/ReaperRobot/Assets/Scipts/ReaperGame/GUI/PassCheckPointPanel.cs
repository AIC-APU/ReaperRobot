using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace smart3tene.Reaper
{
    public class PassCheckPointPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _passCheckPointPanel;

        private CancellationTokenSource _cancellationTokenSource = new();

        private void Awake()
        {
            _passCheckPointPanel.SetActive(false);

            ReaperEventManager.CheckPointPassEvent += ShowPanel;
        }

        private void OnDisable()
        {
            ReaperEventManager.CheckPointPassEvent -= ShowPanel;
        }

        private void ShowPanel()
        {
            _cancellationTokenSource.Cancel();

            _ = ShowPanelTask(_cancellationTokenSource.Token);
        }

        private async UniTask ShowPanelTask(CancellationToken ct)
        {
            _passCheckPointPanel.SetActive(true);

            await UniTask.Delay(TimeSpan.FromSeconds(1));

            _passCheckPointPanel.SetActive(false);
        }
    }

}
