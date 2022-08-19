using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading;
using Cysharp.Threading.Tasks;
using System;
using UniRx;

namespace smart3tene.Reaper
{
    public class TextPopupPanel : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _text;
        [SerializeField, Range(1, 100)] private int _fadeInSpeed = 5;
        [SerializeField, Range(1, 100)] private int _fadeOutSpeed = 5;
        [SerializeField, Range(1, 10)] private int _popupTime = 3;
        #endregion

        #region Private Fields
        private CancellationTokenSource _cancellationTokenSource;
        private UniTask _introductionTask;

        private Queue<string> _textQueue = new();
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            _canvasGroup.alpha = 0;

            ReaperEventManager.TextPopupEvent += ShowPanel;
        }

        private async void Update()
        {
            if(_textQueue.Count != 0)
            {
                await _introductionTask;

                _introductionTask = ShowPanelTask(_textQueue.Dequeue(), _cancellationTokenSource.Token);
            }
        }

        private void OnDisable()
        {
            _cancellationTokenSource.Cancel();

            ReaperEventManager.TextPopupEvent -= ShowPanel;
        }
        #endregion

        #region Private method
        private void ShowPanel(string text)
        {
            _textQueue.Enqueue(text);
        }

        private async UniTask ShowPanelTask(string text, CancellationToken ct = default)
        {
            _text.text = text;

            //フェードで表示
            while (_canvasGroup.alpha < 1)
            {
                _canvasGroup.alpha += (float)_fadeInSpeed / 1000f;
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            //数秒待つ
            await UniTask.Delay(TimeSpan.FromSeconds(_popupTime));

            //フェードで非表示
            while (_canvasGroup.alpha > 0)
            {
                _canvasGroup.alpha -= (float)_fadeOutSpeed / 1000f;
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
        }
        #endregion
    }

}