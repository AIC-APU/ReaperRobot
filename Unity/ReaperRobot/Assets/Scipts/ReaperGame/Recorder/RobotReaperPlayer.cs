using System.Collections.Generic;
using UniRx;
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

        //UIに表示するためのパラメータ
        public IReadOnlyReactiveProperty<float> InputH => _inputH;
        private ReactiveProperty<float> _inputH = new(0);

        public IReadOnlyReactiveProperty<float> InputV => _inputV;
        private ReactiveProperty<float> _inputV = new(0);

        public IReadOnlyReactiveProperty<bool> Lift => _lift;
        private ReactiveProperty<bool> _lift = new(true);

        public IReadOnlyReactiveProperty<bool> Cutter => _cutter;
        private ReactiveProperty<bool> _cutter = new(true);
        #endregion

        #region MonoBehaviour Callbacks
        private void FixedUpdate()
        {
            if (!_isPlaying.Value) return;
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
            _reaperManager.Move(0, 0);

            _csvData.Clear();
            _csvData.AddRange(GetCSVData(filePath));
            if (_csvData.Count == 0)
            {
                return;
            }   

            //値の初期化
            _isPlaying.Value = false;
            PlayTime = 0;
            _flameCount = 0;

            _inputH.Value = 0;
            _inputV.Value = 0;
            _lift.Value = true;
            _cutter.Value = true;

            //pathの初期化
            foreach (var obj in _pathObjects)
            {
                Destroy(obj);
            }
            _pathObjects.Clear();

            //リフト・カッターの初期設定
            _reaperManager.MoveLift(ExtractLift(_csvData, PlayTime));
            _reaperManager.RotateCutter(ExtractCutter(_csvData, PlayTime));
        }

        public override void Play()
        {
            _isPlaying.Value = true;

            StartPlayEvent?.Invoke();
        }

        public override void Pause()
        {
            _isPlaying.Value = false;
            _reaperManager.Move(0, 0);

            PausePlayEvent?.Invoke();
        }

        public override void Stop()
        {
            _reaperManager.Move(0, 0);

            _csvData.Clear();

            //値の初期化
            PlayTime = 0;
            _isPlaying.Value = false;
            _flameCount = 0;

            //pathの初期化
            foreach (var obj in _pathObjects)
            {
                Destroy(obj);
            }
            _pathObjects.Clear();

            StopEvent?.Invoke();
        }

        public override void Back()
        {
            //値の初期化
            _isPlaying.Value = false;
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
            _inputH.Value = input.x;
            _inputV.Value = input.y;
            _reaperManager.Move(input.x, input.y);

            //リフトの上下
            _lift.Value = ExtractLift(data, seconds);
            _reaperManager.MoveLift(_lift.Value);

            //カッターの静動
            _cutter.Value = ExtractCutter(data, seconds);
            _reaperManager.RotateCutter(_cutter.Value);
        }
        #endregion
    }

}
