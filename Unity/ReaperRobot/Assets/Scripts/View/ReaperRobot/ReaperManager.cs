using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Plusplus.ReaperRobot.Scripts.View.ReaperRobot
{
    public class ReaperManager : MonoBehaviour
    {
        #region Serialized Private Field
        [Header("Reaper")]
        [SerializeField] private GameObject _reaper;

        [Header("Cutter")]
        [SerializeField] private Transform _cutterL;
        [SerializeField] private Transform _cutterR;

        [Header("Wheel Collider")]
        [SerializeField] private List<WheelCollider> _wheelColliderL = new();
        [SerializeField] private List<WheelCollider> _wheelColliderR = new();

        [Header("Crawler")]
        [SerializeField] private Animator _crawlerL;
        [SerializeField] private Animator _crawlerR;

        [Header("Parameter")]
        [SerializeField] private ReaperParameter _params;
        #endregion

        #region I Read Only Reactive Property
        public IReadOnlyReactiveProperty<float> InputH => _inputH;
        public IReadOnlyReactiveProperty<float> InputV => _inputV;
        public IReadOnlyReactiveProperty<bool> IsCutting => _isCutting;

        /// <summary>
        /// 0~1の範囲で、リフトの角度の割合を返す。0がリフトが下がっている状態、1がリフトが上がっている状態
        /// </summary>
        public IReadOnlyReactiveProperty<float> LiftAngleRate => _liftAngleRate;
        #endregion

        #region Private
        //カッター&リフト関連
        private ReactiveProperty<bool> _isCutting = new(true);
        private ReactiveProperty<float> _liftAngleRate = new(0f);
        private CancellationTokenSource _cutterCancellationTokenSource = new();
        private float _nowCutterSpeed = 0f;

        //タイヤのアニメーション関連
        private ReactiveProperty<int> _leftRpm = new(0);
        private ReactiveProperty<int> _rightRpm = new(0);

        //入力関連
        private ReactiveProperty<float> _inputH = new(0);
        private ReactiveProperty<float> _inputV = new(0);
        #endregion

        #region Readonly field
        readonly float _maxLiftAngle = 20f;
        readonly float _minLiftAngle = 0f;
        readonly float _liftSpeed = 10f;
        #endregion


        #region MonoBehaviour Callbacks
        private void Start()
        {
            var rigidbody = GetComponent<Rigidbody>();

            //重心の購読
            _params.CenterOfGravity.Subscribe(x => rigidbody.centerOfMass = x).AddTo(this);

            //質量の購読
            _params.RobotMath.Subscribe(x => rigidbody.mass = x).AddTo(this);

            //減衰率の購読
            _params.DampingRate.Subscribe(x => SetWheelDanpingRate(x)).AddTo(this);

            //摩擦の購読
            _params.ForwardFriction.Subscribe(x => SetWheelForwardFriction(x)).AddTo(this);
            _params.SidewaysFriction.Subscribe(x => SetWheelSidewaysFriction(x)).AddTo(this);

            //crawlerアニメーションの処理
            //素のrpmは値が大きすぎるので、直進時の最大rpm = 70f で除算している
            _leftRpm.Subscribe(x => _crawlerL.SetFloat("WheelTorque", (float)x / 70f)).AddTo(this);
            _rightRpm.Subscribe(x => _crawlerR.SetFloat("WheelTorque", (float)x / 70f)).AddTo(this);


            //カッターの処理
            _isCutting
                .Subscribe(isRotate =>
                {
                    _cutterCancellationTokenSource?.Cancel();
                    _cutterCancellationTokenSource = new();
                    AsyncRotateCutter(isRotate, _cutterCancellationTokenSource.Token).Forget();

                    //タグの変更
                    if (isRotate)
                    {
                        _reaper.tag = "Cutting";
                    }
                    else
                    {
                        _reaper.tag = "Untagged";
                    }
                })
                .AddTo(this);
        }

        private void Update()
        {
            _leftRpm.Value = (int)_wheelColliderL[0].rpm;
            _rightRpm.Value = (int)_wheelColliderR[0].rpm;
        }

        private void OnDestroy()
        {
            //非同期処理の停止            
            _cutterCancellationTokenSource?.Cancel();

            _wheelColliderL.Clear();
            _wheelColliderR.Clear();
        }
        #endregion


        #region public method
        /// <summary>
        /// Reaperロボットを移動させる関数。FixedUpdateのタイミングで呼ぶこと
        /// </summary>
        /// <param name="horizontal">水平方向の入力。-1~+1の範囲</param>
        /// <param name="vertical">垂直方向の入力。-1~+1の範囲</param>
        public void Move(float horizontal, float vertical)
        {
            if (_wheelColliderL.Count == 0 || _wheelColliderR.Count == 0) return;
            
            if (horizontal == 0 && vertical == 0)
            {
                _wheelColliderL.ForEach(x => x.brakeTorque = _params.BrakeTorque.Value);
                _wheelColliderR.ForEach(x => x.brakeTorque = _params.BrakeTorque.Value);
            }
            else
            {
                _wheelColliderL.ForEach(x => x.brakeTorque = 0);
                _wheelColliderR.ForEach(x => x.brakeTorque = 0);
            }

            //入力値の範囲を制限
            horizontal = Mathf.Clamp(horizontal, -1, 1);
            vertical = Mathf.Clamp(vertical, -1, 1);

            //入力値を記録
            _inputH.Value = horizontal;
            _inputV.Value = vertical;

            //左右車輪のトルクを計算
            var torqueL = _params.MoveTorque.Value * vertical;
            var torqueR = _params.MoveTorque.Value * vertical;

            if (horizontal > 0)
            {
                torqueR *= 1f - 2f * horizontal;
            }
            else if (horizontal < 0)
            {
                torqueL *= 1f + 2f * horizontal;
            }


            if (_isCutting.Value)
            {
                torqueL *= _params.TorqueRateAtCutting.Value;
                torqueR *= _params.TorqueRateAtCutting.Value;
            }

            _wheelColliderL.ForEach(x => x.motorTorque = torqueL);
            _wheelColliderR.ForEach(x => x.motorTorque = torqueR);
        }

        public void MoveLift(bool isDown)
        {
            if (isDown && GetConvertedLocalAngleX(_reaper.transform) > _minLiftAngle)
            {
                //リフトを下げる処理
                _reaper.transform.Rotate(_liftSpeed * Time.deltaTime, 0, 0);
            }
            else if (!isDown && GetConvertedLocalAngleX(_reaper.transform) < _maxLiftAngle)
            {
                //リフトを上げる処理
                _reaper.transform.Rotate(-_liftSpeed * Time.deltaTime, 0, 0);
            }

            var rate = GetConvertedLocalAngleX(_reaper.transform) / _maxLiftAngle;
            _liftAngleRate.Value = Mathf.Clamp(rate, 0f, 1f);

            //reaperの角度が、0度を起点に±180度になるように変換するローカルメゾット（真上が+90度）
            static float GetConvertedLocalAngleX(Transform reaper)
            {
                var reaperAngleX = reaper.localEulerAngles.x;
                return reaperAngleX >= 180f ? 360f - reaperAngleX : -reaperAngleX;
            }
        }

        public void MoveLift(float angle)
        {
            //角度を指定してリフトを上昇下降させる機能を実装するかも。とりあえず保留
            throw new System.NotImplementedException();
        }

        public void RotateCutter(bool isRotate)
        {
            _isCutting.Value = isRotate;
        }
        #endregion


        #region private method
        /// <summary>
        ///  isCuttingがtrueならカッターを回転させる、falseなら回転を止める非同期処理
        /// </summary>
        private async UniTaskVoid AsyncRotateCutter(bool isCutting, CancellationToken ct = default)
        {
            var maxRotateSpeed = 1000f;
            var minRotateSpeed = 0f;
            var acceleration = 3f;
            while (true)
            {
                //刃の速度を加速（上限下限でおさえる）
                _nowCutterSpeed += isCutting ? acceleration : -acceleration;
                _nowCutterSpeed = Mathf.Clamp(_nowCutterSpeed, minRotateSpeed, maxRotateSpeed);

                //回転
                _cutterL.Rotate(0, _nowCutterSpeed * Time.deltaTime, 0);
                _cutterR.Rotate(0, -_nowCutterSpeed * Time.deltaTime, 0);

                await UniTask.Yield(PlayerLoopTiming.Update, ct);

                //刃が止まったらループを抜ける
                if (!isCutting && _nowCutterSpeed == 0) break;
            }
        }

        private void SetWheelDanpingRate(float dampingRate)
        {
            //Danpingの設定
            _wheelColliderL.ForEach(x => x.wheelDampingRate = dampingRate);
            _wheelColliderR.ForEach(x => x.wheelDampingRate = dampingRate);
        }

        private void SetWheelForwardFriction(float stiffness)
        {
            //前輪の摩擦の設定
            _wheelColliderL.ForEach(x =>
            {
                var forwardFriction = x.forwardFriction;
                forwardFriction.stiffness = stiffness;
                x.forwardFriction = forwardFriction;
            });

            _wheelColliderR.ForEach(x =>
            {
                var forwardFriction = x.forwardFriction;
                forwardFriction.stiffness = stiffness;
                x.forwardFriction = forwardFriction;
            });
        }

        private void SetWheelSidewaysFriction(float stiffness)
        {
            //後輪の摩擦の設定
            _wheelColliderL.ForEach(x =>
            {
                var sidewaysFriction = x.sidewaysFriction;
                sidewaysFriction.stiffness = stiffness;
                x.sidewaysFriction = sidewaysFriction;
            });

            _wheelColliderR.ForEach(x =>
            {
                var sidewaysFriction = x.sidewaysFriction;
                sidewaysFriction.stiffness = stiffness;
                x.sidewaysFriction = sidewaysFriction;
            });
        }
        #endregion
    }
}

