using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

namespace smart3tene.Reaper
{
    public class ShadowReaperPlayer : BaseCSVPlayer
    {
        #region Public Fields
        public Vector3 RepositionPos = new(0f, 0f, 0f);
        public Vector3 RepositionAng = new(0f, 0f, 0f);
        #endregion

        #region Serialized Private Fields
        [SerializeField] private GameObject _shadowPrefab;
        [SerializeField] private ReaperManager _reaperManager;
        [SerializeField] private Transform _reaperTransform;
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

        private Vector3 _startPos = Vector3.zero;
        private Vector3 _startAng = Vector3.zero;
        private Vector3 _offsetPos = Vector3.zero;
        private Vector3 _offsetAng = Vector3.zero;

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
            _shadowTransform = _shadowInstance.transform;

            //pathの初期化
            foreach (var obj in _pathObjects)
            {
                Destroy(obj);
            }
            _pathObjects.Clear();

            //ロボットの初期位置の保存
            _startPos = _reaperTransform.position;
            _startAng = _reaperTransform.eulerAngles;

            _offsetPos = _startPos - ExtractPosition(_csvData, 0f);
            _offsetAng = _startAng - new Vector3(0, ExtractAngleY(_csvData, 0f), 0);

            _shadowTransform.SetPositionAndRotation(_startPos, Quaternion.Euler(_startAng));

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

            _startPos = Vector3.zero;
            _startAng = Vector3.zero;
            _offsetPos= Vector3.zero;
            _offsetAng = Vector3.zero;

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
            _shadowTransform.SetPositionAndRotation(_startPos, Quaternion.Euler(_startAng));
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

        public async void RepositionRobot()
        {
            _reaperManager.Move(0, 0);

            await UniTask.Yield();

            _reaperTransform.SetPositionAndRotation(RepositionPos, Quaternion.Euler(RepositionAng));
            _reaperManager.MoveLift(true);
            _reaperManager.RotateCutter(true);
        }
        #endregion

        #region Private and Protected method
        protected override void OneFlameMove(List<string[]> data, float seconds)
        {
            //UIに表示するために値を取得
            var input = ExtractInput(data, seconds);
            _inputH.Value = input.x;
            _inputV.Value = input.y;

            var rawpos = ExtractPosition(data, seconds);
            _posX.Value = rawpos.x + _offsetPos.x;
            _posY.Value = rawpos.y + _offsetPos.y;
            _posZ.Value = rawpos.z + _offsetPos.z;
            var pos = new Vector3(_posX.Value, _posY.Value, _posZ.Value);

            var rawAngleY = ExtractAngleY(data, seconds);
            _angleY.Value = rawAngleY + _offsetAng.y;
            var rot = Quaternion.Euler(0, _angleY.Value, 0);
            
            _lift.Value   = ExtractLift(data, seconds);
            _cutter.Value = ExtractCutter(data, seconds);

            //影にデータ反映させる
            _shadowTransform.SetPositionAndRotation(pos, rot);
            _shadowManager.MoveLift(_lift.Value);
            _shadowManager.RotateCutter(_cutter.Value);
        }
        #endregion
    }
     
}