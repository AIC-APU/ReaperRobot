using Zenject;
using Plusplus.ReaperRobot.Scripts.Model;
using Plusplus.ReaperRobot.Scripts.Data;

public class RoadInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container
            .Bind<IRoadModel>()
            .To<RoadModel>()
            .AsSingle();

        Container
            .Bind<IRoadData>()
            .To<RoadFromCSV>()
            .AsSingle();
    }
}