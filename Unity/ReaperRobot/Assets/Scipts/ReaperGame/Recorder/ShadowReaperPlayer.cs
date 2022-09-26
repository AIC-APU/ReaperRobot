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
        private ShadowReaperManager _shadowManager;

        private bool _isFastForward = false;
        private bool _isRewind = false;
        #endregion

        #region MonoBehaviour Callbacks
        private void Update ()
        {
            if (!_isPlaying) return;
            if (PlayTime > ExtractSeconds(_csvData, _csvData.Count - 1)) return;

            if (_isFastForward)
            {
                //早送りモードなら時間を倍進める
                PlayTime += Time.deltaTime * 2f;
            }
            else if (_isRewind)
            {
                PlayTime -= Time.deltaTime;
                if(PlayTime < 0) PlayTime = 0f;
            }
            else
            {
                PlayTime += Time.deltaTime;
            }

            OneFlameMove(_csvData, PlayTime);

            if(PlayTime > ExtractSeconds(_csvData, _csvData.Count - 1))
            {
                //再生が終わった処理
                Pause();
                EndCSVEvent?.Invoke();
            }
        }
        #endregion

        #region Public Method
        public override void SetUp(string filePath)
        {
            _csvData.Clear();
            _csvData.AddRange(GetCSVData(filePath));

            if(_csvData.Count == 0)
            {
                return;
            }

            PlayTime = 0f;

            var pos = ExtractPosition(_csvData, PlayTime);
            var angle = ExtractQuaternion(_csvData, PlayTime);

            _shadowInstance = Instantiate(_shadowPrefab, pos, angle);
            _shadowManager = _shadowInstance.GetComponent<ShadowReaperManager>();
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

            PlayTime = 0f;

            _csvData.Clear();

            StopEvent?.Invoke();
        }

        public override void Back()
        {
            if (_csvData.Count == 0)
            {
                return;
            }

            PlayTime = 0f;

            _shadowInstance.transform.position = ExtractPosition(_csvData, PlayTime);
            _shadowInstance.transform.rotation = ExtractQuaternion(_csvData, PlayTime);
            _shadowManager.MoveLift(ExtractLift(_csvData, PlayTime));
            _shadowManager.RotateCutter(ExtractCutter(_csvData, PlayTime));

            _isPlaying = false;
        }

        public void FastForward(bool isFast)
        {
            _isFastForward = isFast;
        }

        public void Rewind(bool isRewind)
        {
            _isRewind = isRewind;
        }
        #endregion

        #region Private and Protected method
        protected override void OneFlameMove(List<string[]> data, float seconds)
        {
            _shadowInstance.transform.position = ExtractPosition(data, seconds);
            _shadowInstance.transform.rotation = ExtractQuaternion(data, seconds);
            _shadowManager.MoveLift(ExtractLift(data, seconds));
            _shadowManager.RotateCutter(ExtractCutter(data, seconds));
        }
        #endregion
    }

}