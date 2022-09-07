using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;
using UniRx.Triggers;

namespace smart3tene.Reaper
{
    public class GameTimeText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timeNum;

        private void Awake()
        {
            ReaperEventManager.ResetEvent += ResetTime;

            this.UpdateAsObservable()
                .Subscribe(_ => _timeNum.text = GameTimer.GetCurrentTimeSpan.ToString(@"hh\:mm\:ss"))
                .AddTo(this);

            GameTimer.Start();
        }

        private void OnDestroy()
        {
            ReaperEventManager.ResetEvent -= ResetTime;
        }

        private void ResetTime()
        {
            GameTimer.Restart();
        }
    }
}

