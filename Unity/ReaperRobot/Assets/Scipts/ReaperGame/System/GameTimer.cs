using System.Diagnostics;

namespace smart3tene.Reaper
{
    public static class GameTimer
    {
        private static Stopwatch _stopWatch = new();

        public static bool IsTimerRunning => _stopWatch.IsRunning;
        public static float GetCurrentSeconds => (float)_stopWatch.Elapsed.TotalSeconds;
        public static System.TimeSpan GetCurrentTimeSpan => _stopWatch.Elapsed;

        public static void Start()
        {
            _stopWatch.Start();
        }

        public static float Stop()
        {
            _stopWatch.Stop();
            return (float)_stopWatch.Elapsed.TotalSeconds;
        }

        public static void Reset()
        {
            _stopWatch.Reset();
        }

        public static void Restart()
        {
            _stopWatch.Restart();
        }
    }

}
