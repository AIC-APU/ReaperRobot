using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;

namespace smart3tene.Reaper
{
    public class SaveFilePanel : MonoBehaviour
    {
        [SerializeField] private GameObject _saveFilePanel;
        [SerializeField] private TMP_Text _filePathText;

        private CancellationTokenSource _savePanelCancelTaken;
        private UniTask _saveFileTask;

        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = _saveFilePanel.GetComponent<CanvasGroup>();

            _savePanelCancelTaken = new CancellationTokenSource();
            ReaperEventManager.SaveFileEvent += OnSaveFileEvent;
        }
        private void OnDestroy()
        {
            ReaperEventManager.SaveFileEvent -= OnSaveFileEvent;
        }

        private async void OnSaveFileEvent(string fileName)
        {
            await _saveFileTask;

            _saveFileTask = ShowSaveFilePanel(fileName, _savePanelCancelTaken.Token);
        }

        private async UniTask ShowSaveFilePanel(string fileName, CancellationToken ct = default)
        {
            _filePathText.text = $"{fileName} was saved";

            //フェードで表示
            while (_canvasGroup.alpha < 1)
            {
                _canvasGroup.alpha += 0.05f;
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            //数秒待つ
            await UniTask.Delay(TimeSpan.FromSeconds(2));

            //フェードで非表示
            while (_canvasGroup.alpha > 0)
            {
                _canvasGroup.alpha -= 0.05f;
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
        }
    }

}
