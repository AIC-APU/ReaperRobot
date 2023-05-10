using System.Collections.Generic;
using UnityEngine;
using Plusplus.ReaperRobot.Scripts.Model;

namespace Plusplus.ReaperRobot.Scripts.Data
{
    public class SaveAsCSV : ISaveData
    {
        readonly string _directory = Application.streamingAssetsPath + "/../../../CSVData";
        public void Save(IEnumerable<ReaperDataSet> dataSets, string fileName)
        {            
            //ヘッダーの作成
            string csvData = string.Join(",", ReaperDataSet.Label()) + "\n";

            //データの変換
            foreach (var data in dataSets)
            {
                csvData += string.Join(",", data.ToStringArray()) + "\n";
            }

            //ファイル名の作成
            string filePath = _directory + "/" + fileName + ".csv";

            //ファイルの書き込み
            CSVUtility.Write(filePath, csvData);
        }
    }
}
