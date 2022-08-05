using Cysharp.Threading.Tasks;
using Photon.Pun;
using System.Threading;
using UniRx;
using UnityEngine;

namespace smart3tene.Reaper
{
    public class ReaperManager : MonoBehaviourPun
    {
        #region Serialized Private Field
        [Header("Reaper")]
        [SerializeField] private GameObject _reaper;

        [Header("Cutter")]
        [SerializeField] private Transform _cutterL;
        [SerializeField] private Transform _cutterR;

        [Header("Wheel Collider")]
        [SerializeField] private WheelCollider _wheelColliderL2;
        [SerializeField] private WheelCollider _wheelColliderL3;
        [SerializeField] private WheelCollider _wheelColliderR2;
        [SerializeField] private WheelCollider _wheelColliderR3;

        [Header("Crawler")]
        [SerializeField] private Animator _crawlerL;
        [SerializeField] private Animator _crawlerR;
        #endregion


        #region private & readonly Field
        //カッター&リフト関連
        public IReadOnlyReactiveProperty<bool> IsCutting => _isCutting;
        private ReactiveProperty<bool> _isCutting = new(true);
        public IReadOnlyReactiveProperty<bool> IsLiftDown => _isLiftDown;
        private ReactiveProperty<bool> _isLiftDown = new(true);

        private CancellationTokenSource _liftCancellationTokenSource = new();
        private CancellationTokenSource _cutterCancellationTokenSource = new();
        private float _nowCutterSpeed = 0f;


        //Wheel Collider関連
        readonly float rotateTorqueMultiplier = 100f;
        readonly float moveTorqueMultiplier = 300f;
        readonly float brakeTorque = 500f;

        public IReadOnlyReactiveProperty<float> LeftRpm => _leftRpm;
        private ReactiveProperty<float> _leftRpm = new(0);
        public IReadOnlyReactiveProperty<float> RightRpm => _rightRpm;
        private ReactiveProperty<float> _rightRpm = new(0);
        #endregion


        #region MonoBehaviour Callbacks
        private void Start()
        {
            if (PhotonNetwork.IsConnected && !photonView.IsMine) return;

            RotateCutter(_isCutting.Value);
            MoveLift(_isLiftDown.Value);
        }

        private void Update()
        {
            //crawlerアニメーションの処理
            //素のrpmは値が大きすぎるので、直進時の最大rpm（計測値）で除算している
            _leftRpm.Value = _wheelColliderL2.rpm;
            _rightRpm.Value = _wheelColliderR2.rpm;

            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC(nameof(RPCPlayCrawlerAnimation), RpcTarget.All, _leftRpm.Value, _rightRpm.Value);
            }
            else
            {
                RPCPlayCrawlerAnimation(_leftRpm.Value, _rightRpm.Value);
            }
        }

        private void OnDestroy()
        {
            //非同期処理の停止            
            _liftCancellationTokenSource?.Cancel();
            _cutterCancellationTokenSource?.Cancel();

            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.RemoveRPCs(PhotonNetwork.LocalPlayer);
            }        
        }
        #endregion


        #region public method
        /// <summary>
        /// Reaperロボットを移動させる関数。FixedUpdateのタイミングで呼ぶこと
        /// </summary>
        /// <param name="horizontal">水平方向の入力。-1~+1の範囲</param>
        /// <param name="vertical">垂直方向の入力。-1~+1の範囲</param>
        public async UniTaskVoid AsyncMove(float horizontal, float vertical)
        {
            //この処理はFixedUpdateのタイミングで行う
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate);

            //入力値の範囲を制限
            horizontal = Mathf.Clamp(horizontal, -1, 1);
            vertical = Mathf.Clamp(vertical, -1, 1);

            //左右車輪のトルクを計算
            var torqueL = moveTorqueMultiplier * vertical;
            var torqueR = moveTorqueMultiplier * vertical;

            torqueL += rotateTorqueMultiplier * horizontal;
            torqueR -= rotateTorqueMultiplier * horizontal;

            if (_isCutting.Value)
            {
                torqueL /= 2f;
                torqueR /= 2f;
            }

            _wheelColliderL2.motorTorque = torqueL;
            _wheelColliderL3.motorTorque = torqueL;
            _wheelColliderR2.motorTorque = torqueR;
            _wheelColliderR3.motorTorque = torqueR;

            //モーター音

        }

        public void PutOnBrake()
        {
            if (PhotonNetwork.IsConnected && !photonView.IsMine) return;

            _wheelColliderL2.brakeTorque = brakeTorque;
            _wheelColliderL3.brakeTorque = brakeTorque;
            _wheelColliderR2.brakeTorque = brakeTorque;
            _wheelColliderR3.brakeTorque = brakeTorque;
        }

        public void ReleaseBrake()
        {
            if (PhotonNetwork.IsConnected && !photonView.IsMine) return;

            _wheelColliderL2.brakeTorque = 0;
            _wheelColliderL3.brakeTorque = 0;
            _wheelColliderR2.brakeTorque = 0;
            _wheelColliderR3.brakeTorque = 0;
        }

        public void MoveLift(bool isDown)
        {
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC(nameof(RPCMoveLift), RpcTarget.All, isDown);
            }
            else
            {
                RPCMoveLift(isDown);
            }
        }
        
        public void RotateCutter(bool isRotate)
        {         
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC(nameof(RPCRotateCutter), RpcTarget.All, isRotate);
            }
            else
            {
                RPCRotateCutter(isRotate);
            }
        }
        #endregion


        #region private method
        /// <summary>
        /// isCuttingがtrueならリフトを下げる、falseなら上げる非同期処理
        /// </summary>
        private async UniTaskVoid AsyncMoveLift(bool isDown, CancellationToken ct = default)
        {
            var reaperTransform = _reaper.transform;
            var liftSpeed = 10f;
            if (isDown)
            {
                //0度までリフトを下げるためのループ
                while (GetConvertedLocalAngleX(reaperTransform) > 0)
                {
                    reaperTransform.Rotate(liftSpeed * Time.deltaTime, 0, 0);
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }
            }
            else
            {
                //20度までリフトを上げるためのループ
                while (GetConvertedLocalAngleX(reaperTransform) < 20)
                {
                    reaperTransform.Rotate(-liftSpeed * Time.deltaTime, 0, 0);
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }
            }

            //reaperの角度が、0度を起点に±180度になるように変換するローカルメゾット（真上が+90度）
            static float GetConvertedLocalAngleX(Transform reaper)
            {
                var reaperAngleX = reaper.localEulerAngles.x;
                return reaperAngleX >= 180f ? 360f - reaperAngleX : -reaperAngleX;
            }
        }

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

                //モーター音


                await UniTask.Yield(PlayerLoopTiming.Update, ct);

                //刃が止まったらループを抜ける
                if (!isCutting && _nowCutterSpeed == 0) break;
            }
        }

        [PunRPC]
        public void RPCRotateCutter(bool isRotate)
        {
            _cutterCancellationTokenSource?.Cancel();
            _cutterCancellationTokenSource = new();

            //オブジェクトの回転
            AsyncRotateCutter(isRotate, _cutterCancellationTokenSource.Token).Forget();

            //フラグの変更
            _isCutting.Value = isRotate;

            //タグの変更
            if (isRotate)
            {
                _reaper.tag = "Cutting";
            }
            else
            {
                _reaper.tag = "Untagged";
            }
        }

        [PunRPC]
        public void RPCMoveLift(bool isDown)
        {
            _liftCancellationTokenSource?.Cancel();
            _liftCancellationTokenSource = new();
            AsyncMoveLift(isDown, _liftCancellationTokenSource.Token).Forget();

            _isLiftDown.Value = isDown;
        }

        [PunRPC]
        public void RPCPlayCrawlerAnimation(float leftRpm, float rightRpm)
        {
            //crawlerアニメーションの処理
            //素のrpmは値が大きすぎるので、直進時の最大rpm（計測値）で除算している
            _crawlerL.SetFloat("WheelTorque", leftRpm / 70);
            _crawlerR.SetFloat("WheelTorque", rightRpm / 70);
        }
        #endregion
    }
}

