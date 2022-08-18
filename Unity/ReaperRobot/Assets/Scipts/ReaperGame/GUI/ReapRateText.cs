using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;

namespace smart3tene.Reaper
{
    public class ReapRateText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _reapRateNum;

        private void Awake()
        {
            GrassCounter.CutGrassCount.Subscribe(_ => _reapRateNum.text = GrassCounter.CutGrassPercent.Value.ToString("F1"));
        }
    }
}