using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace smart3tene.Reaper
{
    public class CheckPointManager : MonoBehaviour
    {
        #region Public Property
        public IReadOnlyReactiveProperty<string> Introduction => _intro;
        private ReactiveProperty<string> _intro = new("");
        #endregion

        #region Serialized Private Fields
        [SerializeField] private List<BaseCheckPoint> _checkPointList = new();
        #endregion

        #region Private Fields
        private CancellationTokenSource _cancellationTokenSource = new();
        #endregion

        #region MonoBehaviour Callbacks
        private void Start()
        {
            ReaperEventManager.AllCheckPointPassEvent += AllCheckPointPass;

            GameTimer.Start();

            _ = CheckPointFlow(_cancellationTokenSource.Token);
        }

        private void OnDisable()
        {
            _cancellationTokenSource.Cancel();
            ReaperEventManager.AllCheckPointPassEvent -= AllCheckPointPass;
        }
        #endregion

        #region Private method
        private async UniTask CheckPointFlow(CancellationToken ct = default)
        {
            for(int i = 0; i < _checkPointList.Count; i++)
            {
                _checkPointList[i].SetUp();

                await UniTask.WaitUntil(() => _checkPointList[i].IsChecked.Value, PlayerLoopTiming.Update, ct);

                ReaperEventManager.InvokeCheckPointPassEvent();
            }

            //すべてのチェックポイントを通過したらイベント発生
            ReaperEventManager.InvokeAllCheckPointPassEvent();
        }

        private void AllCheckPointPass()
        {
            GameTimer.Stop();
            Debug.Log("clear!");
        }

        private void ResetCheckPoint()
        {

        }
        #endregion
    }

}