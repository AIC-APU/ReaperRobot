using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;

namespace smart3tene.Reaper
{
    public class CheckPointSystem : MonoBehaviour
    {
        #region Public Fields

        #endregion

        #region Serialized Private Fields
        [SerializeField] private List<GameObject> _checkPointList = new();
        #endregion

        #region Private Fields
        [SerializeField] private ReactiveProperty<int> _checkIndex = new(0);

        private CancellationTokenSource _cancelTokenSource = new();
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            //チェックポイントを通過したら、チェックポイントを消してindexを増加
            foreach (GameObject checkPoint in _checkPointList)
            {
                var check = checkPoint.GetComponent<CheckPoint>();
                check.IsChecked.Subscribe(x =>
                {
                    if (x)
                    {
                        checkPoint.SetActive(false);
                        _checkIndex.Value++;
                    }
                });
            }

            //checkIndexが増加する度にクリアかどうか調べる
            _checkIndex.Subscribe(x =>
            {
                if (x == _checkPointList.Count)
                {
                    //クリア: イベントを発生
                    ReaperEventManager.InvokeAllCheckPointPassEvent();
                }
                else
                {
                    //未クリア: 次のチェックポイントを表示
                    _checkPointList[x].SetActive(true);
                }
            });

            ReaperEventManager.AllCheckPointPathEvent += AllCheckPointPass;

            ResetCheckPoints();
        }

        private void OnDisable()
        {
            _cancelTokenSource.Cancel();
            ReaperEventManager.AllCheckPointPathEvent -= AllCheckPointPass;
        }
        #endregion

        #region Private method

        private void ResetCheckPoints()
        {
            foreach (GameObject checkPoint in _checkPointList)
            {
                checkPoint.SetActive(false);
            }

            _checkIndex.Value = 0;
            _checkPointList[0].SetActive(true);

            //各チェックポイントのリセットもしないといけない

        }

        private void AllCheckPointPass()
        {
            GameTimer.Stop();
            Debug.Log("clear!");
        }
        #endregion
    }

}