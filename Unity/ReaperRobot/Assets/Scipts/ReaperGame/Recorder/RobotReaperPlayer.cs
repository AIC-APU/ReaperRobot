using System.Collections.Generic;
using UnityEngine;

namespace smart3tene.Reaper
{
    public class RobotReaperPlayer : BaseCSVPlayer
    {
        #region Serialized Private Fields
        [SerializeField] private ReaperManager _reaperManager;
        [SerializeField] private Transform _reaperTransform;
        [SerializeField] private ReaperController _controller;
        #endregion

        #region MonoBehaviour Callbacks
        private void Update()
        {
            if (!_isPlaying) return;
            if (PlayTime > ExtractSeconds(_csvData, _csvData.Count - 1)) return;

            PlayTime += Time.deltaTime;

            if (_isFastForward) 
            {
                //早送りモードなら時間を倍進める
                PlayTime += Time.deltaTime;
            } 

            OneFlameMove(_csvData, PlayTime);

            if (PlayTime > ExtractSeconds(_csvData, _csvData.Count - 1))
            {
                //再生が終わった処理
                Pause();
                EndCSVEvent?.Invoke();
            }
        }
        #endregion

        #region Public method
        public override void SetUp()
        {
            if (_csvData.Count == 0)
            {
                return;
            }

            _reaperManager.Move(0, 0);
            PlayTime = 0;

            //Position,Rotationにはcsvを使わない方がいいかも
            //実機の操作量を使用する時に参照できないから
            _reaperTransform.position = ExtractPosition(_csvData, PlayTime);
            _reaperTransform.rotation = ExtractQuaternion(_csvData, PlayTime);

            _reaperManager.MoveLift(ExtractLift(_csvData, PlayTime));
            _reaperManager.RotateCutter(ExtractCutter(_csvData, PlayTime));

            _controller.enabled = false;
        }

        public override void Play()
        {
            _isPlaying = true;

            StartPlayEvent?.Invoke();
        }

        public override void Pause()
        {
            _isPlaying = false;
            _reaperManager.Move(0, 0);

            //Position,Rotationにはcsvを使わない方がいいかも
            //実機の操作量を使用する時に参照できないから
            _reaperTransform.position = ExtractPosition(_csvData, PlayTime);
            _reaperTransform.rotation = ExtractQuaternion(_csvData, PlayTime);

            _reaperManager.MoveLift(ExtractLift(_csvData, PlayTime));
            _reaperManager.RotateCutter(ExtractCutter(_csvData, PlayTime));

            PausePlayEvent?.Invoke();
        }

        public override void Stop()
        {
            _isPlaying = false;
            _controller.enabled = true;
            _reaperManager.Move(0, 0);

            PlayTime = 0;
            _csvData.Clear();

            StopEvent?.Invoke();
        }

        public override void Back()
        {
            _isPlaying = false;
            SetUp();   
        }

        public override void FastForward(bool isFast)
        {
            _isFastForward = isFast;
        }
        #endregion

        #region Private method
        protected override void OneFlameMove(List<string[]> data, float seconds)
        {
            var input = ExtractInput(data, seconds);
            _reaperManager.Move(input.x, input.y);

            //リフトの上下
            _reaperManager.MoveLift(ExtractLift(data, seconds));

            //カッターの静動
            _reaperManager.RotateCutter(ExtractCutter(data, seconds));
        }
        #endregion
    }

}
