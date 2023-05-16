using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Zenject;

namespace Plusplus.ReaperRobot.Scripts.Model
{
    public class SaveModel: ISaveModel
    {
        [Inject] ISaveData _saveData;
        public UniTask Save(IEnumerable<ReaperDataSet> dataSets, string fileName) => _saveData.Save(dataSets, fileName);
    }
}
