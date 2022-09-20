using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;

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
        private void FixedUpdate()
        {
            if (!_isPlaying) return;
            if (_csvIndex >= _csvData.Count) return;

            OneFlameMove(_csvData, _csvIndex);

            PlayTime += Time.fixedDeltaTime;

            if (_csvIndex == _csvData.Count - 1)
            {
                //再生が終わった処理
                Pause();
                EndCSVEvent?.Invoke();
            }

            _csvIndex++;

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

            _csvIndex = 1;

            _reaperTransform.position = ExtractPosition(_csvData, _csvIndex);
            _reaperTransform.rotation = ExtractQuaternion(_csvData, _csvIndex);
            _reaperManager.MoveLift(ExtractLift(_csvData, _csvIndex));
            _reaperManager.RotateCutter(ExtractCutter(_csvData, _csvIndex));

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

            _reaperTransform.position = ExtractPosition(_csvData, _csvIndex);
            _reaperTransform.rotation = ExtractQuaternion(_csvData,_csvIndex);
            _reaperManager.MoveLift(ExtractLift(_csvData, _csvIndex));
            _reaperManager.RotateCutter(ExtractCutter(_csvData, _csvIndex));

            PausePlayEvent?.Invoke();
        }

        public override void Stop()
        {
            _isPlaying = false;
            _controller.enabled = true;
            _reaperManager.Move(0, 0);

            PlayTime = 0;
            _csvData.Clear();
            _csvIndex = 1;

            StopEvent?.Invoke();
        }

        public override void Back()
        {
            _isPlaying = false;
            SetUp();   
        }
        #endregion

        #region Private method
        protected override void OneFlameMove(List<string[]> data, int index)
        {
            var input = ExtractInput(data, index);
            _reaperManager.Move(input.x, input.y);

            //リフトの上下
            _reaperManager.MoveLift(ExtractLift(data, index));

            //カッターの静動
            _reaperManager.RotateCutter(ExtractCutter(data, index));
        }
        #endregion
    }

}
