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
    [DefaultExecutionOrder(-1)]
    public class CheckPointIntroductionPanel : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField] private CheckPointManager _checkPointManager;
        [SerializeField] private CanvasGroup _checkPointIntrodecution;
        [SerializeField] private TMP_Text _introductionText;
        #endregion

        #region Private Fields
        private CancellationTokenSource _cancellationTokenSource;
        private UniTask _introductionTask;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            _checkPointIntrodecution.alpha = 0;

            _checkPointManager.Introduction.Skip(1).Subscribe(x => ShowPanel(x));
        }

        private void OnDisable()
        {
            _cancellationTokenSource.Cancel();
        }
        #endregion

        #region Private method
        private async void ShowPanel(string intro)
        {
            await _introductionTask;

            _introductionTask = ShowPanelTask(intro, _cancellationTokenSource.Token);
        }

        private async UniTask ShowPanelTask(string intro, CancellationToken ct = default)
        {
            _introductionText.text = intro;

            //フェードで表示
            while (_checkPointIntrodecution.alpha < 1)
            {
                _checkPointIntrodecution.alpha += 0.005f;
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            //数秒待つ
            await UniTask.Delay(TimeSpan.FromSeconds(3));

            //フェードで非表示
            while (_checkPointIntrodecution.alpha > 0)
            {
                _checkPointIntrodecution.alpha -= 0.005f;
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
        }
        #endregion
    }

}