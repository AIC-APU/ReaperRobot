using UniRx;

namespace smart3tene.Reaper
{
    public static class GrassCounter
    {
        public static IReadOnlyReactiveProperty<int> AllGrassCount => _allGrassCount;
        private static ReactiveProperty<int> _allGrassCount = new(0);

        public static IReadOnlyReactiveProperty<int> CutGrassCount => _cutGrassCount;
        private static ReactiveProperty<int> _cutGrassCount = new(0);

        public static void AddAllGrass() => _allGrassCount.Value++;

        public static void AddCutGrass() => _cutGrassCount.Value++;

        public static void MinusCutGrass() => _cutGrassCount.Value--;

        public static float CutGrassPercent() => _allGrassCount.Value == 0 ? 0 : 100f * (float)_cutGrassCount.Value / (float)_allGrassCount.Value;

        public static int RemainingGrass() => _allGrassCount.Value - _cutGrassCount.Value;
    }
}

