using System;
using System.Collections.Generic;
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
        private Vector3 _offsetPosition;
        private Vector3 _offsetAngle;
        private IDisposable _disposable;
        #endregion

        #region MonoBehaviour Callbacks
        void OnDestroy()
        {
            _disposable?.Dispose();
        }
        #endregion

        #region Public method
        public override void InitializeReplay(string filePath)
        {
            //データの読み込み
            _dataSets.Clear();
            _dataSets.AddRange(GetDataSets(filePath));

            //位置と角度のオフセットを取得
            _offsetPosition = _target.transform.position - ExtractPosition(_dataSets, 0f);
            _offsetAngle = _target.transform.eulerAngles - new Vector3(0, ExtractAngleY(_dataSets, 0f), 0);

            _disposable =
                _timer
                .Time
                .Subscribe(seconds =>
                {
                    Replay(_dataSets, seconds);
                });
        }

        public override void FinalizeReplay()
        {
            _disposable?.Dispose();
            _dataSets.Clear();
        }
        #endregion

        #region Private method
        protected override void Replay(List<DataSet> data, float seconds)
        {
            var rawPos = ExtractPosition(data, seconds);
            var posx = rawPos.x + _offsetPosition.x;
            var posy = rawPos.y + _offsetPosition.y;
            var posz = rawPos.z + _offsetPosition.z;
            var pos = new Vector3(posx, posy, posz);

            var rawAngleY = ExtractAngleY(data, seconds);
            var angleY = rawAngleY + _offsetAngle.y;
            var rot = Quaternion.Euler(0, angleY, 0);

            _target.transform.SetPositionAndRotation(pos, rot);
        }
        #endregion
    }
}
