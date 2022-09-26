using System;
using System.Diagnostics;
using UnityEngine;

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

        /// <summary>
        /// {hours}:{miniutes}:{seconds}(:{milliseconds})の文字列を返す
        /// </summary>
        /// <returns></returns>
        public static string ConvertSecondsToString(float seconds, bool includeMilliSec = true)
        {
            var hour = Mathf.FloorToInt(seconds / 3600f);
            var min = Mathf.FloorToInt((seconds - hour * 3600f) / 60f);
            var sec = Mathf.FloorToInt((seconds - hour * 3600f - min * 60f));
            var msec = Mathf.FloorToInt((seconds % 1f) * 1000f);

            string time;
            if (includeMilliSec)
            {
                //00:00:00:000 の形式で返す
                time = $"{hour:D2}:{min:D2}:{sec:D2}:{msec:D3}";
            }
            else
            {
                //00:00:00 の形式で返す
                time = $"{hour:D2}:{min:D2}:{sec:D2}";
            }
            return time;
        }

        public static string ConvertTimeSpanToString(TimeSpan timeSpan, bool includeMilliSec = true)
        {
            string time;
            if (includeMilliSec)
            {
                time = timeSpan.ToString(@"hh\:mm\:ss\:fff");
            }
            else
            {
                time = timeSpan.ToString(@"hh\:mm\:ss");
            }
            return time;
        }
    }

}
