using System;
using System.Diagnostics;
using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.UnityComponent.Timer
{
    public static class TimeConverter
    {
        /// <summary>
        /// {hours}:{miniutes}:{seconds}(:{milliseconds})の文字列を返す
        /// </summary>
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

        /// <summary>
        /// {hours}:{miniutes}:{seconds}(:{milliseconds})の文字列を返す
        /// </summary>
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
