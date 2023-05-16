using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Zenject;

namespace Plusplus.ReaperRobot.Scripts.Model
{
    public class RoadModel : IRoadModel
    {
        [Inject] readonly IRoadData _roadData;

        public async UniTask<List<ReaperDataSet>> Road(string filePath)
        {
            return await _roadData.Road(filePath);
        }
    }
}
