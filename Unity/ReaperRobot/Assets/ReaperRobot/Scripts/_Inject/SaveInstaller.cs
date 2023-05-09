using Zenject;
using Plusplus.ReaperRobot.Scripts.Model;
using Plusplus.ReaperRobot.Scripts.Data;
public class SaveInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container
            .Bind<ISaveModel>()
            .To<SaveModel>()
            .AsSingle();
            
        Container
            .Bind<ISaveData>()
            .To<SaveAsCSV>()
            .AsSingle();
    }
}