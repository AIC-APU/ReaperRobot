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
            this.UpdateAsObservable()
                .Subscribe(_ => _timeNum.text = GameTimer.GetCurrentTimeSpan.ToString(@"hh\:mm\:ss"))
                .AddTo(this);
        }
    }
}

