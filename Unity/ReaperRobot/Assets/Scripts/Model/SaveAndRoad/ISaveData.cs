using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace Plusplus.ReaperRobot.Scripts.Model
{
    public interface ISaveData
    {
        public UniTask Save(IEnumerable<ReaperDataSet> dataSets, string fileName);
    }
}
