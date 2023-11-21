using UnityEngine;
using UniRx;

namespace Plusplus.ReaperRobot.Scripts.View.Replay
{
    public class TransformReplay : BaseReplay
    {
        #region Serialized Private Fields
        [SerializeField] private Transform _target;
        #endregion

        #region Private Fields
        private Vector3 _defaultPosition;
        private Vector3 _defaultAngle;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            _defaultPosition = _target.transform.position;
            _defaultAngle = _target.transform.eulerAngles;
            
            _replayManager
                .Time
                .Where(_ => _replayManager.IsDataReady)
                .Subscribe(_ =>
                {
                    Replay();
                })
                .AddTo(this);
        }
        #endregion

        #region Private method
        protected override void Replay()
        {
            var rawPos = _replayManager.GetPosition();
            var offsetPos = _defaultPosition - _replayManager.GetStartPosition();
            var pos = rawPos + offsetPos;

            var rawAngleY = _replayManager.GetAngleY();
            var offsetAngle = _defaultAngle - new Vector3(0,_replayManager.GetStartAngleY(),0);
            var angleY = rawAngleY + offsetAngle.y;
            var rot = Quaternion.Euler(0, angleY, 0);

            _target.transform.SetPositionAndRotation(pos, rot);
        }
        #endregion
    }
}
