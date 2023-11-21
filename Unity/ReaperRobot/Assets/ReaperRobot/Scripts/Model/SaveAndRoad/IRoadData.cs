using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace Plusplus.ReaperRobot.Scripts.Model
{
    public interface IRoadData
    {
        public UniTask<List<ReaperDataSet>> Road(string filePath);
    }
}
