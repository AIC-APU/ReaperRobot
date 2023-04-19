using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

namespace Plusplus.ReaperRobot.Scripts.UnityComponent.CheckPoint
{
    public class CheckPointManager : MonoBehaviour
    {
        #region Event
        public UnityEvent OnCheckPointPass;
        private void InvokeCheckPointPass() => OnCheckPointPass?.Invoke();
        public UnityEvent OnAllCheckPointsPass;
        private void InvokeAllCheckPointPass() => OnAllCheckPointsPass?.Invoke();
        public UnityEvent<string> OnPopupIntroduction;
        private void InvokePopupIntroduction(string intro) => OnPopupIntroduction?.Invoke(intro);
        #endregion

        #region Serialized Private Fields
        [SerializeField] private List<BaseCheckPoint> _checkPointList = new();
        #endregion

        #region Private Fields
        private CancellationTokenSource _cancellationTokenSource = new();
        #endregion

        #region MonoBehaviour Callbacks
        void Start()
        {
            _ = CheckPointFlow(_cancellationTokenSource.Token);
        }

        void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
        }
        #endregion

        #region Private method
        private async UniTask CheckPointFlow(CancellationToken ct = default)
        {
            foreach (BaseCheckPoint checkPoint in _checkPointList)
            {
                //チェックポイントの初期化
                checkPoint.InitializeCheckPoint();

                //チェックポイントの説明の表示
                InvokePopupIntroduction(checkPoint.Introduction);

                //チェックポイントを通過するまで待機
                await UniTask.WaitUntil(() => checkPoint.IsChecked.Value, PlayerLoopTiming.Update, ct);

                //チェックポイントの通過イベント発生
                InvokeCheckPointPass();

                //チェックポイントの後処理
                checkPoint.FinalizeCheckPoint();
            }

            //すべてのチェックポイントを通過したらイベント発生
            InvokeAllCheckPointPass();
        }
        #endregion
    }
}
