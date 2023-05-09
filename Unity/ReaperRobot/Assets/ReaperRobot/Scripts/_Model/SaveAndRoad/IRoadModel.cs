using System.Collections.Generic;

namespace Plusplus.ReaperRobot.Scripts.Model
{
    public interface IRoadModel
    {
        List<ReaperDataSet> Road(string filePath);
    }
}
