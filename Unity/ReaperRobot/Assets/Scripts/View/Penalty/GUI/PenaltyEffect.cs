using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.View.Penalty
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PenaltyEffect : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField,Range(0,1)] private float _popupTime = 0.1f;
        [SerializeField,Range(0,1)] private float _fadeOutTime = 0.3f;
        #endregion

        #region Private Fields
        private CanvasGroup _canvasGroup;
        #endregion

        #region MonoBehaviour Callbacks
        async void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;

            await UniTask.WaitUntil(() => PenaltyManager.Instance != null);
            PenaltyManager.Instance.PenaltyEvent += PopupPenaltyEffectPanel;
        }

        private void OnDestroy()
        {
            PenaltyManager.Instance.PenaltyEvent -= PopupPenaltyEffectPanel;
        }
        #endregion

        #region Private method
        private async void PopupPenaltyEffectPanel()
        {
            //ペナルティ発生時、一瞬この画面を表示
            _canvasGroup.alpha = 1f;

            //数秒待つ
            await UniTask.Delay(TimeSpan.FromSeconds(_popupTime), false, PlayerLoopTiming.Update);

            //フェードで非表示にする
            var time = _fadeOutTime;
            while (_canvasGroup.alpha > 0)
            {
                time -= Time.deltaTime;
                time = Mathf.Clamp(time, 0, _fadeOutTime);
                _canvasGroup.alpha = time / _fadeOutTime;
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }
        #endregion
    }

}