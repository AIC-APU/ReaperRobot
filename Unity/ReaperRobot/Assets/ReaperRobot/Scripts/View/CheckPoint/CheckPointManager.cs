using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Plusplus.ReaperRobot.Scripts.View.CheckPoint
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
        private CancellationTokenSource _cancellationTokenSource;
        #endregion

        #region MonoBehaviour Callbacks
        void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            CheckPointFlow(_cancellationTokenSource.Token).Forget();
        }

        void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
        }
        #endregion

        #region Private method
        private async UniTaskVoid CheckPointFlow(CancellationToken ct = default)
        {
            for(int i = 0; i < _checkPointList.Count; i++)
            {
                //チェックポイントの初期化
                _checkPointList[i].InitializeCheckPoint();

                //チェックポイントの説明の表示
                InvokePopupIntroduction(_checkPointList[i].Introduction);

                //チェックポイントを通過するまで待機
                await UniTask.WaitUntil(() => _checkPointList[i].IsChecked.Value, PlayerLoopTiming.Update, ct);

                //チェックポイントの後処理
                _checkPointList[i].FinalizeCheckPoint();

                //チェックポイントの通過イベント発生
                if(i < _checkPointList.Count - 1) InvokeCheckPointPass();
            }

            //すべてのチェックポイントを通過したらイベント発生
            InvokeAllCheckPointPass();
        }
        #endregion
    }
}
