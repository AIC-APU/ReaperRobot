using System.Collections.Generic;

namespace Plusplus.ReaperRobot.Scripts.Model
{
    public interface ISaveModel
    {
        void Save(IEnumerable<ReaperDataSet> dataSets, string fileName);
    }
}
