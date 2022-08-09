using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

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
        private ReactiveProperty<int> _checkIndex = new(0);

        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
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
        }

        private void AllCheckPointPass()
        {
            GameTimer.Stop();
            Debug.Log("clear!");
        }
        #endregion
    }

}