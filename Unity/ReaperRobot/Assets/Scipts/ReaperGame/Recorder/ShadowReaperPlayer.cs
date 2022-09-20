using System;
using System.Collections.Generic;
using UnityEngine;

namespace smart3tene.Reaper
{
    public class ShadowReaperPlayer : BaseCSVPlayer
    {
        #region Serialized Private Fields
        [SerializeField] private GameObject _shadowPrefab;
        #endregion

        #region Private Fields
        private GameObject _shadowInstance;
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

        #region Public Method
        public override void SetUp()
        {
            if(_csvData.Count == 0)
            {
                return;
            }

            _csvIndex = 1;
            PlayTime = 0;

            var pos = ExtractPosition(_csvData, _csvIndex);
            var angle = Quaternion.Euler(0, ExtractAngleY(_csvData, _csvIndex), 0);

            _shadowInstance = Instantiate(_shadowPrefab, pos, angle);
        }

        public override void Play()
        {
            _isPlaying = true;

            StartPlayEvent?.Invoke();
        }

        public override void Pause()
        {
            _isPlaying = false;

            PausePlayEvent?.Invoke();
        }

        public override void Stop()
        {
            _isPlaying = false;
            if(_shadowInstance != null) Destroy(_shadowInstance);

            PlayTime = 0;
            _csvData.Clear();
            _csvIndex = 1;

            StopEvent?.Invoke();
        }

        public override void Back()
        {
            if (_csvData.Count == 0)
            {
                return;
            }

            _csvIndex = 1;
            PlayTime = 0;

            _shadowInstance.transform.position = ExtractPosition(_csvData, _csvIndex);
            _shadowInstance.transform.rotation = Quaternion.Euler(0, ExtractAngleY(_csvData, _csvIndex), 0);

            _isPlaying = false;
        }
        #endregion

        #region Private and Protected method
        protected override void OneFlameMove(List<string[]> data, int index)
        {
            //得たデータを使い影の位置を毎フレーム更新
            //線形補正とか考えるならここ
            _shadowInstance.transform.position = ExtractPosition(data, index);
            _shadowInstance.transform.rotation = Quaternion.Euler(0, ExtractAngleY(data, index), 0);
        }
        #endregion
    }

}