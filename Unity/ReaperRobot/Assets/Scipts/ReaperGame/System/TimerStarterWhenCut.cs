using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UniRx;

namespace smart3tene.Reaper 
{
    public class TimerStarterWhenCut : MonoBehaviour
    {
        private int _lastCutCount = 0;

        void Awake()
        {
            GrassCounter.CutGrassCount.Subscribe(x =>
            {
                if (_lastCutCount == 0 && x == 1)
                {
                    GameTimer.Start();
                }

                _lastCutCount = x;

            }).AddTo(this);

            ReaperEventManager.ResetEvent += ResetTimer;
        }

        private void OnDestroy()
        {
            ReaperEventManager.ResetEvent -= ResetTimer;
        }

        private void ResetTimer()
        {
            GameTimer.Reset();
            GameTimer.Stop();
        }
    }

}