using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;

namespace Plusplus.ReaperRobot.Scripts.View.Grass
{
    public class ReapRateText : MonoBehaviour
    {
        [SerializeField] private GrassCounter _grassCounter;
        [SerializeField] private TMP_Text _reapRateNum;

        private void Awake()
        {
            _grassCounter
                .ObserveEveryValueChanged(counter => counter.CutGrasspercent)
                .Subscribe(percent => _reapRateNum.text = percent.ToString("F1"))
                .AddTo(this);
        }
    }
}