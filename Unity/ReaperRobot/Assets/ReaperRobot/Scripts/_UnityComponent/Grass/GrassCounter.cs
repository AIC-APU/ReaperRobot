using UnityEngine;
using UnityEngine.Events;
using UniRx;

namespace Plusplus.ReaperRobot.Scripts.UnityComponent.Grass
{
    public class GrassCounter : MonoBehaviour
    {
        #region Unity Events
        [SerializeField] private UnityEvent OnFirstGrassCut;
        [SerializeField] private UnityEvent OnAllGrassCut;
        #endregion

        #region Public Properties
        public int AllGrassCount => _allGrassCount;
        public int CutGrassCount => _cutGrassCount;
        public bool IsAllGrassCut => _allGrassCount == _cutGrassCount;
        public float CutGrassRatio => _allGrassCount == 0 ? 0f : (float)_cutGrassCount / _allGrassCount;
        public float CutGrasspercent => CutGrassRatio * 100f;
        #endregion

        #region Private Fields
        private int _allGrassCount = 0;
        private int _cutGrassCount = 0;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            Grass[] grasses = FindObjectsOfType<Grass>();
            _allGrassCount = grasses.Length;
            foreach (Grass grass in grasses)
            {
                grass
                .IsCut
                .Skip(1)
                .Subscribe(isCut =>
                {
                    if (isCut)
                    {
                        _cutGrassCount++;
                        if (_cutGrassCount == 1)
                        {
                            OnFirstGrassCut?.Invoke();
                            Debug.Log("OnFirstGrassCut");
                        }
                        if (IsAllGrassCut)
                        {
                            OnAllGrassCut?.Invoke();
                            Debug.Log("OnAllGrassCut");
                        }
                    }
                    else
                    {
                        _cutGrassCount--;
                    }
                })
                .AddTo(this);
            }
        }
        #endregion
    }
}