using UniRx;
using UnityEngine;

namespace smart3tene.Reaper
{
    public class TimerStarterWhenCut : MonoBehaviour
    {
        private int _lastCutCount = 0;

        void Awake()
        {
            //草が1つカットされた時にタイマースタート
            GrassCounter.CutGrassCount
                .Subscribe(x =>
                {
                    if (_lastCutCount == 0 && x == 1)
                    {
                        GameTimer.Start();
                    }

                    _lastCutCount = x;

                })
                .AddTo(this);

            ReaperEventManager.ResetEvent += ResetTimer;
            ResetTimer();
        }

        private void OnDestroy()
        {
            ReaperEventManager.ResetEvent -= ResetTimer;
            ResetTimer();
        }

        private void ResetTimer()
        {
            GameTimer.Reset();
            GameTimer.Stop();
        }
    }

}