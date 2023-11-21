using System;
using UniRx;
using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.View.CheckPoint
{
    public class MoveCheckPoint : BaseCheckPoint
    {
        #region Serialized Private Fields
        [SerializeField] private Transform _target;
        [SerializeField, Range(0.1f, 10)] private float _goalMoveDistance = 1.0f;
        #endregion

        #region Private Fields
        private Vector3 _startPosition;
        private IDisposable _disposable;
        #endregion

        #region MonoBehaviour Callbacks
        void OnDestroy()
        {
            _disposable?.Dispose();
        }
        #endregion

        #region Public method
        public override void InitializeCheckPoint()
        {
            _startPosition = _target.position;
            _disposable =
                _target
                .ObserveEveryValueChanged(x => x.position)
                .Subscribe(position =>
                {
                    if (Vector3.Distance(_startPosition, position) >= _goalMoveDistance)
                    {
                        _isChecked.Value = true;
                    }
                });
        }

        public override void FinalizeCheckPoint()
        {
            _disposable?.Dispose();
        }
        #endregion
    }
}
