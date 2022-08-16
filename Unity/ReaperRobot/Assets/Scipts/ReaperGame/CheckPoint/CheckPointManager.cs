using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace smart3tene.Reaper
{
    public class CheckPointManager : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField] private List<BaseCheckPoint> _checkPointList = new();
        #endregion

        #region Private Fields
        private CancellationTokenSource _cancellationTokenSource = new();
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            ReaperEventManager.AllCheckPointPathEvent += AllCheckPointPass;

            GameTimer.Start();

            _ = CheckPointFlow(_cancellationTokenSource.Token);
        }

        private void OnDisable()
        {
            _cancellationTokenSource.Cancel();
            ReaperEventManager.AllCheckPointPathEvent -= AllCheckPointPass;
        }
        #endregion

        #region Private method
        private async UniTask CheckPointFlow(CancellationToken ct = default)
        {
            for(int i = 0; i < _checkPointList.Count; i++)
            {
                _checkPointList[i].SetUp();

                await UniTask.WaitUntil(() => _checkPointList[i].IsChecked.Value, PlayerLoopTiming.Update, ct);
            }

            ReaperEventManager.InvokeAllCheckPointPassEvent();
        }

        private void AllCheckPointPass()
        {
            GameTimer.Stop();
            Debug.Log("clear!");
        }
        #endregion
    }

}