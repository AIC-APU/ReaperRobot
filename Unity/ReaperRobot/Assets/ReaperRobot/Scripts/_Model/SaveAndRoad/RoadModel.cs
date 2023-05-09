using System.Collections.Generic;
using Zenject;

namespace Plusplus.ReaperRobot.Scripts.Model
{
    public class RoadModel : IRoadModel
    {
        [Inject] readonly IRoadData _roadData;

        public List<ReaperDataSet> Road(string filePath)
        {
            return _roadData.Road(filePath);
        }
    }
}
