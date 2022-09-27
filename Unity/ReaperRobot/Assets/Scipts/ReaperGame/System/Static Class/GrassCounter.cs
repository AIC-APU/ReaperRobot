using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace smart3tene.Reaper
{
    public static class GrassCounter
    {
        public static IReadOnlyReactiveProperty<int> AllGrassCount => _allGrassCount;
        private static ReactiveProperty<int> _allGrassCount = new(0);

        public static IReadOnlyReactiveProperty<int> CutGrassCount => _cutGrassCount;
        private static ReactiveProperty<int> _cutGrassCount = new(0);

        public static IReadOnlyReactiveProperty<float> CutGrassPercent => _cutGrassPercent;
        private static ReactiveProperty<float> _cutGrassPercent = new(0);

        public static IReadOnlyReactiveProperty<int> RemainingGrass => _remainingGrass;
        private static ReactiveProperty<int> _remainingGrass = new(0);

        static GrassCounter()
        {
            SceneManager.sceneUnloaded += UnloadReset;
        }       

        public static void AddAllGrass()
        {
            _allGrassCount.Value++;
        }
        
        public static void AddCutGrass()
        {
            _cutGrassCount.Value++;

            UpdateValues();
        }

        public static void MinusCutGrass()
        {
            _cutGrassCount.Value--;

            UpdateValues();
        }

        private static void UpdateValues()
        {
            _cutGrassPercent.Value = _allGrassCount.Value == 0 ? 0 : 100f * (float)_cutGrassCount.Value / (float)_allGrassCount.Value;
            _remainingGrass.Value  = _allGrassCount.Value - _cutGrassCount.Value;
        }

        private static void UnloadReset(Scene thisScene)
        {
            _allGrassCount.Value = 0;
            _cutGrassCount.Value = 0;

            UpdateValues();
        }
    }
}

