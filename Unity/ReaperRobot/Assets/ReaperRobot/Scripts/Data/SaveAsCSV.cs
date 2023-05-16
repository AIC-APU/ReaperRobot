using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Plusplus.ReaperRobot.Scripts.Model;

namespace Plusplus.ReaperRobot.Scripts.Data
{
    public class SaveAsCSV : ISaveData
    {
        readonly string _directory = Application.streamingAssetsPath + "/../../../CSVData";
        public async UniTask Save(IEnumerable<ReaperDataSet> dataSets, string fileName)
        {
            //ファイル名の作成
            string filePath = _directory + "/" + fileName + ".csv";

            //ヘッダーの作成
            string csvData = string.Join(",", ReaperDataSet.Label()) + "\n";

            //データの変換
            await foreach (string csvLine in AsyncJoin(dataSets))
            {
                csvData += csvLine;
            }

            //ファイルの書き込み
            await CSVUtility.Write(filePath, csvData);
        }

        async IAsyncEnumerable<string> AsyncJoin(IEnumerable<ReaperDataSet> dataSets)
        {
            int dataCount = 0;
            foreach(var data in dataSets)
            {
                yield return await UniTask.FromResult(string.Join(",", data.StringArray) + "\n");

                //10件ごとにYield
                dataCount++;
                if(dataCount % 10 == 0) await UniTask.Yield();
            }
        }
    }
}
