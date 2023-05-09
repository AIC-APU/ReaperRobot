using System.Collections.Generic;
using Plusplus.ReaperRobot.Scripts.Model;

namespace Plusplus.ReaperRobot.Scripts.Data
{
    public class RoadFromCSV : IRoadData
    {
        public List<ReaperDataSet> Road(string filePath)
        {
            var csvdata = CSVUtility.Read(filePath);
            var list = new List<ReaperDataSet>();

            //一行目はラベルなので飛ばす
            for(int i = 1; i < csvdata.Count; i++)
            {
                list.Add(new ReaperDataSet(csvdata[i]));
            }
            return list;
        }
    }
}
