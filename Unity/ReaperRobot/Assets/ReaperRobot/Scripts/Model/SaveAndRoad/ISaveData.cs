using System.Collections.Generic;

namespace Plusplus.ReaperRobot.Scripts.Model
{
    public interface ISaveData
    {
        public void Save(IEnumerable<ReaperDataSet> dataSets, string fileName);
    }
}
