using System;
using System.Text.RegularExpressions;

namespace Plusplus.ReaperRobot.Scripts.Model
{
    public static class TimeConverter
    {
        /// <summary>
        /// 秒数を{hours}:{miniutes}:{seconds}(:{milliseconds})の文字列に変換する
        /// </summary>
        public static string ToString(TimeSpan timeSpan, bool includeMilliSec = true)
        {
            return includeMilliSec ? timeSpan.ToString(@"hh\:mm\:ss\:fff") : timeSpan.ToString(@"hh\:mm\:ss");
        }

        /// <summary>
        /// {hours}:{miniutes}:{seconds}(:{milliseconds})の文字列を返す
        /// </summary>
        public static string ToString(float seconds, bool includeMilliSec = true)
        {
            var timespan = TimeSpan.FromSeconds(seconds);
            return ToString(timespan, includeMilliSec);
        }

        /// <summary>
        /// {hours}:{miniutes}:{seconds}(:{milliseconds})の文字列をタイムスパンに変換する
        /// </summary>
        public static TimeSpan ToTimeSpan(string time)
        {
            TimeSpan result = new TimeSpan();
            if (Regex.IsMatch(time, "[0-9]{2}:[0-9]{2}:[0-9]{2}:[0-9]{3}"))
            {
                //00:00:00:000 の時
                var timeArray = time.Split(":");

                var hour = int.Parse(timeArray[0]);
                var min = int.Parse(timeArray[1]);
                var sec = int.Parse(timeArray[2]);
                var msec = timeArray.Length == 4 ? int.Parse(timeArray[3]) : 0;

                result = new TimeSpan(0, hour, min, sec, msec);
            }
            else if (Regex.IsMatch(time, "^[0-9]*\\.?[0-9]+$"))
            {
                //0.000... の時
                var seconds = float.Parse(time);
                result = TimeSpan.FromSeconds(seconds);
            }
            else
            {
                throw new System.ArgumentException();
            }
            return result;
        }

        /// <summary>
        /// {hours}:{miniutes}:{seconds}(:{milliseconds})の文字列をfloat型の秒数に変換する
        /// </summary>
        public static float ToSeconds(string time)
        {
            return ToSeconds(ToTimeSpan(time));
        }
        public static TimeSpan ToTimeSpan(float seconds)
        {
            return TimeSpan.FromSeconds(seconds);
        }
        public static float ToSeconds(TimeSpan timeSpan)
        {
            return (float)timeSpan.TotalSeconds;
        }
    }
}