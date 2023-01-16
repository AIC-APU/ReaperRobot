using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace smart3tene.Reaper
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PenaltyPanel : MonoBehaviour
    {
        #region Private Fields
        private CanvasGroup _canvasGroup;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;

            ReaperEventManager.PenaltyEvent += PopupPenaltyPanel;
        }

        private void OnDestroy()
        {
            ReaperEventManager.PenaltyEvent -= PopupPenaltyPanel;
        }
        #endregion

        #region Private method
        private async void PopupPenaltyPanel()
        {
            //ペナルティ発生時、一瞬この画面を表示
            _canvasGroup.alpha = 1f;

            //数秒待つ
            var waitTime = 0.1f;
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), false, PlayerLoopTiming.Update);

            //フェードで非表示
            var fadeOutTime = 0.3f;
            var time = fadeOutTime;
            while (_canvasGroup.alpha > 0)
            {
                time -= Time.deltaTime;
                time = Mathf.Clamp(time, 0, fadeOutTime);
                _canvasGroup.alpha = time / fadeOutTime;
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }
        #endregion
    }

}