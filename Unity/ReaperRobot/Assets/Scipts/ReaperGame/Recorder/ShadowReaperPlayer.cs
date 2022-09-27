using System.Collections.Generic;
using UnityEngine;

namespace smart3tene.Reaper
{
    public class ShadowReaperPlayer : BaseCSVPlayer
    {
        #region Serialized Private Fields
        [SerializeField] private GameObject _shadowPrefab;
        [SerializeField] private Material _pathMaterial;
        #endregion

        #region Private Fields
        private GameObject _shadowInstance;
        private Transform _shadowTransform;
        private ShadowReaperManager _shadowManager;

        private bool _isFastForward = false;
        private bool _isRewind = false;

        private List<GameObject> _pathObjects = new();
        private int _flameCount = 0;
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

            if(_flameCount % 10 == 0)
            {
                var obj = MeshCreator.CreateCubeMesh(_shadowTransform.position, _pathMaterial, 0.05f);
                _pathObjects.Add(obj);
            }
            _flameCount++;
            

            if (PlayTime > ExtractSeconds(_csvData, _csvData.Count - 1))
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
            _flameCount = 0;

            var pos = ExtractPosition(_csvData, PlayTime);
            var angle = ExtractQuaternion(_csvData, PlayTime);

            _shadowInstance = Instantiate(_shadowPrefab, pos, angle);
            _shadowTransform = _shadowInstance.transform;
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
            _shadowTransform = null;

            PlayTime = 0f;

            _csvData.Clear();

            foreach (var obj in _pathObjects)
            {
                Destroy(obj);
            }
            _pathObjects.Clear();
            _flameCount = 0;

            StopEvent?.Invoke();
        }

        public override void Back()
        {
            if (_csvData.Count == 0)
            {
                return;
            }

            PlayTime = 0f;

            _shadowTransform.SetPositionAndRotation(ExtractPosition(_csvData, PlayTime), ExtractQuaternion(_csvData, PlayTime));
            _shadowManager.MoveLift(ExtractLift(_csvData, PlayTime));
            _shadowManager.RotateCutter(ExtractCutter(_csvData, PlayTime));

            _isPlaying = false;

            foreach(var obj in _pathObjects)
            {
                Destroy(obj);
            }
            _pathObjects.Clear();
            _flameCount = 0;
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
            _shadowTransform.SetPositionAndRotation(ExtractPosition(data, seconds), ExtractQuaternion(data, seconds));
            _shadowManager.MoveLift(ExtractLift(data, seconds));
            _shadowManager.RotateCutter(ExtractCutter(data, seconds));
        }
        #endregion
    }

}