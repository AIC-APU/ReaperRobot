using System.Collections.Generic;
using UnityEngine;

namespace smart3tene.Reaper
{
    public class RobotReaperPlayer : BaseCSVPlayer
    { 
        #region Serialized Private Fields
        [SerializeField] private ReaperManager _reaperManager;
        [SerializeField] private Transform _reaperTransform;
        [SerializeField] private Material _pathMaterial;
        #endregion

        #region Private Fields
        private int _flameCount = 0;
        private List<GameObject> _pathObjects = new();
        #endregion

        #region MonoBehaviour Callbacks
        private void FixedUpdate()
        {
            if (!_isPlaying) return;
            if (PlayTime > ExtractSeconds(_csvData, _csvData.Count - 1)) return;

            PlayTime += Time.fixedDeltaTime;

            OneFlameMove(_csvData, PlayTime);

            //メッシュを使った軌跡の記録
            if (_flameCount % 10 == 0)
            {
                var obj = MeshCreator.CreateCubeMesh(_reaperTransform.position, _pathMaterial, 0.05f);
                _pathObjects.Add(obj);
            }
            _flameCount++;

            //プレイタイムがcsvデータの最後の時間を過ぎていないか確認
            if (PlayTime > ExtractSeconds(_csvData, _csvData.Count - 1))
            {
                //再生が終わった処理
                Pause();
                EndCSVEvent?.Invoke();
            }
        }
        #endregion

        #region Public method
        public override void SetUp(string filePath)
        {
            _csvData.Clear();
            _csvData.AddRange(GetCSVData(filePath));

            if (_csvData.Count == 0)
            {
                return;
            }

            //値の初期化
            _isPlaying = false;
            PlayTime = 0;
            _flameCount = 0;

            //リフト・カッターの初期設定
            _reaperManager.MoveLift(ExtractLift(_csvData, PlayTime));
            _reaperManager.RotateCutter(ExtractCutter(_csvData, PlayTime));
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

            PausePlayEvent?.Invoke();
        }

        public override void Stop()
        {
            _isPlaying = false;
            _reaperManager.Move(0, 0);

            PlayTime = 0;
            _csvData.Clear();

            //pathの初期化
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
            //値の初期化
            _isPlaying = false;
            PlayTime = 0;
            _flameCount = 0;

            //リフト・カッターの初期設定
            _reaperManager.MoveLift(ExtractLift(_csvData, PlayTime));
            _reaperManager.RotateCutter(ExtractCutter(_csvData, PlayTime));

            //pathの初期化
            foreach (var obj in _pathObjects)
            {
                Destroy(obj);
            }
            _pathObjects.Clear();
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
