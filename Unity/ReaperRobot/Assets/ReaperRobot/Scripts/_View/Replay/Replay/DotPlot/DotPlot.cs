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
                .Subscribe(seconds =>
                {
                    if(seconds != 0)
                    {
                        var dot = Plot(_target.position);
                        _dots.Add(dot);
                    }
                    else
                    {
                        //初期状態に戻った時
                        foreach(var dot in _dots)
                        {
                            Destroy(dot);
                        }
                    }
                })
                .AddTo(this);
        }
        #endregion

        #region Private method
        private GameObject Plot(Vector3 position)
        {
            var obj = MeshCreateUtility.CreateCubeMesh(position, _dotMaterial, _size);
            return obj;
        }
        #endregion
    }
}
