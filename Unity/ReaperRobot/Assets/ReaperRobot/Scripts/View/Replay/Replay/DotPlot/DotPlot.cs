using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Plusplus.ReaperRobot.Scripts.View.Replay
{
    public class DotPlot : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private ReplayManager _replayManager;
        [SerializeField] private Transform _target;
        [SerializeField] private Material _dotMaterial;
        [SerializeField] private float _size = 0.1f;
        [SerializeField, Range(0.1f, 3f)] private float _intervalSeconds = 0.1f;
        #endregion

        #region Private Fields
        private List<GameObject> _dots = new List<GameObject>();
        #endregion

        #region MonoBehaviour Callbacks
        void Start()
        {
            _replayManager
                .Time
                .ThrottleFirst(TimeSpan.FromSeconds(_intervalSeconds))
                .Where(_ => _replayManager.IsDataReady)
                .Where(seconds => seconds != 0)
                .Subscribe(seconds =>
                {
                    var dot = Plot(_target.position);
                    _dots.Add(dot);
                })
                .AddTo(this);

            _replayManager.OnReset += ResetPlot;
        }

        void OnDestroy()
        {
            ResetPlot();
            _replayManager.OnReset -= ResetPlot;
        }
        #endregion

        #region Private method
        private GameObject Plot(Vector3 position)
        {
            var obj = MeshCreateUtility.CreateCubeMesh(position, _dotMaterial, _size);
            return obj;
        }

        private void ResetPlot()
        {
            foreach (var dot in _dots)
            {
                Destroy(dot);
            }
            _dots.Clear();
        }
        #endregion
    }
}
