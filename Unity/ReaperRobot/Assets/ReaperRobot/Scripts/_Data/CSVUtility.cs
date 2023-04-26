using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.Data
{
    public static class CSVUtility
    {
         private static readonly string _directory = Application.streamingAssetsPath + "/../../../CSVData";

        public static void CreateFile(string fileName)
        {
            Write(_directory + $"/{fileName}", "");
        }

        public static void Write(string fileName, string data, FileMode fileMode = FileMode.Create)
        {
            if (!fileName.EndsWith(".csv")) fileName += ".csv";
            var filePath = _directory + $"/{fileName}";

            //ディレクトリ作成
            var dir = Path.GetDirectoryName(filePath);
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }
                
            using (var fs = new FileStream(filePath, fileMode))
            using (var writer = new StreamWriter(fs, Encoding.UTF8))
            {
                writer.Write(data);
            }

            #if UNITY_EDITOR
            AssetDatabase.Refresh();
            #endif
        }

        public static void Write(string fileName, List<string[]> dataList, FileMode fileMode = FileMode.Create)
        {
            var stringData = dataList
                .Select(i => string.Join(",", i))
                .Aggregate((i, j) => i + "\n" + j);

            Write(fileName, stringData);
        }

        /// <summary>
        /// CSVファイルを読み込み、二次元配列として返す
        /// </summary>
        public static List<string[]> Read(string filePath)
        {
            if (!filePath.EndsWith(".csv")) filePath += ".csv";

            // 読み込み              
            string csvText = File.ReadAllText(filePath, Encoding.GetEncoding("utf-8"));
            StringReader stringReader = new StringReader(csvText);
            List<string[]> recordList = new List<string[]>();

            while (stringReader.Peek() > -1)
            {
                // 1行を取り出す
                string record = stringReader.ReadLine();

                // カンマ区切りの値を配列に格納
                string[] fields = record.Split(',');

                // リストに追加
                recordList.Add(fields);
            }

            // StringReaderを閉じる
            stringReader.Close();

            // リストで出力
            //recordList[0][1]のように行列で取り出せる。
            return recordList;
        }
    }
}
