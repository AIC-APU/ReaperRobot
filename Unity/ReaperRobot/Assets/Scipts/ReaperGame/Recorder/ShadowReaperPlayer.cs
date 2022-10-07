using System.Collections.Generic;
using TMPro;
using UniRx;
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

        //UIに表示するためのパラメータ
        public IReadOnlyReactiveProperty<float> InputH => _inputH;
        private ReactiveProperty<float> _inputH = new(0);

        public IReadOnlyReactiveProperty<float> InputV => _inputV;
        private ReactiveProperty<float> _inputV = new(0);

        public IReadOnlyReactiveProperty<bool> Lift => _lift;
        private ReactiveProperty<bool> _lift = new(true);

        public IReadOnlyReactiveProperty<bool> Cutter => _cutter;
        private ReactiveProperty<bool> _cutter = new(true);

        public IReadOnlyReactiveProperty<float> PosX => _posX;
        private ReactiveProperty<float> _posX = new(0);

        public IReadOnlyReactiveProperty<float> PosY => _posY;
        private ReactiveProperty<float> _posY = new(0);

        public IReadOnlyReactiveProperty<float> PosZ => _posZ;
        private ReactiveProperty<float> _posZ = new(0);

        public IReadOnlyReactiveProperty<float> Angle => _angleY;
        private ReactiveProperty<float> _angleY = new(0);

        #endregion

        #region MonoBehaviour Callbacks
        private void Update ()
        {
            if (!_isPlaying.Value) return;
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

            _isPlaying.Value = false;
            PlayTime = 0f;
            _flameCount = 0;

            //インスタンス生成
            if(_shadowInstance != null)
            {
                Destroy(_shadowInstance);
            }
            _shadowInstance = Instantiate(_shadowPrefab);

            //pathの初期化
            foreach (var obj in _pathObjects)
            {
                Destroy(obj);
            }
            _pathObjects.Clear();

            //位置の設定・初期化
            _shadowTransform = _shadowInstance.transform;
            _shadowTransform.SetPositionAndRotation(ExtractPosition(_csvData, PlayTime), ExtractQuaternion(_csvData, PlayTime));

            //マネージャーの設定・初期化
            _shadowManager = _shadowInstance.GetComponent<ShadowReaperManager>();
            _shadowManager.MoveLift(ExtractLift(_csvData, PlayTime));
            _shadowManager.RotateCutter(ExtractCutter(_csvData, PlayTime));
        }

        public override void Play()
        {
            _isPlaying.Value = true;

            StartPlayEvent?.Invoke();
        }

        public override void Pause()
        {
            _isPlaying.Value = false;

            PausePlayEvent?.Invoke();
        }

        public override void Stop()
        {
            _csvData.Clear();

            _isPlaying.Value = false;
            PlayTime = 0f;
            _flameCount = 0;

            if (_shadowInstance != null) Destroy(_shadowInstance);
            _shadowTransform = null;           

            foreach (var obj in _pathObjects)
            {
                Destroy(obj);
            }
            _pathObjects.Clear();

            StopEvent?.Invoke();
        }

        public override void Back()
        {
            if (_csvData.Count == 0)
            {
                return;
            }

            _isPlaying.Value = false;
            PlayTime = 0f;
            _flameCount = 0;

            //位置とカッターとリフトの初期化
            _shadowTransform.SetPositionAndRotation(ExtractPosition(_csvData, PlayTime), ExtractQuaternion(_csvData, PlayTime));
            _shadowManager.MoveLift(ExtractLift(_csvData, PlayTime));
            _shadowManager.RotateCutter(ExtractCutter(_csvData, PlayTime));

            //軌跡の削除
            foreach(var obj in _pathObjects)
            {
                Destroy(obj);
            }
            _pathObjects.Clear();
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
            //UIに表示するために値を取得
            var input = ExtractInput(data, seconds);
            _inputH.Value = input.x;
            _inputV.Value = input.y;

            var pos = ExtractPosition(data, seconds);
            _posX.Value = pos.x;
            _posY.Value = pos.y;
            _posZ.Value = pos.z;

            _angleY.Value = ExtractAngleY(data, seconds);
            _lift.Value   = ExtractLift(data, seconds);
            _cutter.Value = ExtractCutter(data, seconds);

            //影にデータ反映させる
            _shadowTransform.SetPositionAndRotation(pos, ExtractQuaternion(data, seconds));
            _shadowManager.MoveLift(_lift.Value);
            _shadowManager.RotateCutter(_cutter.Value);
        }
        #endregion
    }

}