using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.View.Web
{
    public static class QueryReaderUtility
    {
        /// <summary>
        /// 現在のURLのクエリパラメータから指定したキーの値を取得する. キーが存在しない場合は空文字を返す.
        /// </summary>
        public static string GetValue(string key)
        {
            var queries = GetQueryDictionary();

            if (queries.ContainsKey(key))
            {
                return queries[key];
            }
            else
            {
                return "";
            }
        }

        private static Dictionary<string, string> GetQueryDictionary()
        {
            var uri = new Uri(Application.absoluteURL);
            var queryStr = uri.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped);

            var queries = queryStr
                .Split('&')
                .Select(q => q.Split('='))
                .Where(x => x.Length == 2)
                .ToDictionary(x => x[0], x => x[1]);

            return queries;
        }
    }
}
