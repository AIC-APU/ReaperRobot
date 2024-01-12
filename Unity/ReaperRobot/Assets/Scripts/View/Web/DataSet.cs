using System.Linq;

namespace Plusplus.ReaperRobot.Scripts.View.Web
{
    public class DataSet
    {
        readonly string[] sceneList = {"Field", "Training", "Slope"};

        public DataSet(string id, string scene, int time, int penalty, int total, string rank)
        {
            if(!sceneList.Contains(scene)) throw new System.ArgumentOutOfRangeException(nameof(scene));
            if(time <= 0) throw new System.ArgumentOutOfRangeException(nameof(time));
            if(penalty < 0) throw new System.ArgumentOutOfRangeException(nameof(penalty));
            if(total <= 0) throw new System.ArgumentOutOfRangeException(nameof(total));
            
            Id = id;
            Scene = scene;
            Time = time;
            Penalty = penalty;
            Total = total;
            Rank = rank;
        }

        public string Id { get; }
        public string Scene { get; }
        public int Time { get; }
        public int Penalty { get; }
        public int Total { get; }
        public string Rank { get; }

        public string ToJson()
        {
            return $"{{\"id\":\"{Id}\",\"scene\":\"{Scene}\",\"time\":{Time},\"penalty\":{Penalty},\"total\":{Total},\"rank\":\"{Rank}\"}}";
        }
    }
}
