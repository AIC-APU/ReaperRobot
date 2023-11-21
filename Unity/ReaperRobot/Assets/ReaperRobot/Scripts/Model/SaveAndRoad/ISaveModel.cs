using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace Plusplus.ReaperRobot.Scripts.Model
{
    public interface ISaveModel
    {
        UniTask Save(IEnumerable<ReaperDataSet> dataSets, string fileName);
    }
}
