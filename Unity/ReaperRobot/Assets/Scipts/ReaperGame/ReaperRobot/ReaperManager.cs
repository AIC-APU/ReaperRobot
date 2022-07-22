using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
using Photon.Pun;

namespace smart3tene.Reaper
{
    public class ReaperManager : MonoBehaviourPun
    {
        #region Public Field
        public Transform reaperCameraTransform;
        #endregion

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
        private bool _isOperatable = true;
        private bool _isCameraOperatable = true;

        //カメラ
        public IReadOnlyReactiveProperty<Vector3> CameraOffsetPos => _cameraOffsetPos;
        private ReactiveProperty<Vector3> _cameraOffsetPos = new();

        public IReadOnlyReactiveProperty<Vector3> CameraOffsetRot => _cameraOffsetRot;
        private ReactiveProperty<Vector3> _cameraOffsetRot = new();

        readonly Vector3 cameraDefaultOffsetPos = new(0f, 1.2f, -0.5f);
        readonly Vector3 cameraDefaultOffsetRot = new(30f, 0f, 0f);


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

            ResetCameraPos();
            SetCameraTransform();
            RotateCutter(_isCutting.Value);
            MoveLift(_isLiftDown.Value);

            if (GameSystem.Instance != null)
            {
                GameSystem.Instance.NowViewMode.Subscribe(x =>
                {
                    if (x == GameSystem.ViewMode.REAPER)
                    {
                        _isOperatable = true;
                        _isCameraOperatable = true;
                    }
                    else if (x == GameSystem.ViewMode.FPV || x == GameSystem.ViewMode.VR)
                    {
                        _isOperatable = true;
                        _isCameraOperatable = false;
                    }
                    else
                    {
                        _ = AsyncMove(0, 0);
                        _isOperatable = false;
                    }
                });
            }
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

        private void LateUpdate()
        {
            //カメラ位置
            SetCameraTransform();
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
            if (!_isOperatable) return;

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

            _wheelColliderL2.motorTorque = torqueL;
            _wheelColliderL3.motorTorque = torqueL;
            _wheelColliderR2.motorTorque = torqueR;
            _wheelColliderR3.motorTorque = torqueR;

            //モーター音

        }

        public void PutOnBrake()
        {
            if (!_isOperatable) return;
            if (PhotonNetwork.IsConnected && !photonView.IsMine) return;

            _wheelColliderL2.brakeTorque = brakeTorque;
            _wheelColliderL3.brakeTorque = brakeTorque;
            _wheelColliderR2.brakeTorque = brakeTorque;
            _wheelColliderR3.brakeTorque = brakeTorque;
        }

        public void ReleaseBrake()
        {
            if (!_isOperatable) return;
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

        public void ResetCameraPos()
        {
            if (!_isOperatable || !_isCameraOperatable) return;

            _cameraOffsetPos.Value = cameraDefaultOffsetPos;
            _cameraOffsetRot.Value = cameraDefaultOffsetRot;
        }

        public void MoveCamera(float x, float y, float z)
        {
            if (!_isOperatable || !_isCameraOperatable) return;

            _cameraOffsetPos.Value += new Vector3(x, y, z);

            var clampedVec = _cameraOffsetPos.Value;

            clampedVec.x = Mathf.Clamp(clampedVec.x, -1f, 1f);
            clampedVec.y = Mathf.Clamp(clampedVec.y, 0.5f, 2f);
            clampedVec.z = Mathf.Clamp(clampedVec.z, -2f, 2f);

            _cameraOffsetPos.Value = clampedVec;
        }

        public void RotateCamera(float x, float y, float z)
        {
            if (!_isOperatable || !_isCameraOperatable) return;

            _cameraOffsetRot.Value += new Vector3(x, y, z);

            var clampedVec = _cameraOffsetRot.Value;

            clampedVec.x = Mathf.Clamp(clampedVec.x, -90f, 90f);
            clampedVec.y = Mathf.Clamp(clampedVec.y, -90f, 90f);
            clampedVec.z = Mathf.Clamp(clampedVec.z, -90f, 90f);

            _cameraOffsetRot.Value = clampedVec;
        }
        #endregion


        #region private method
        /// <summary>
        /// isCuttingがtrueならリフトを下げる、falseなら上げる非同期処理
        /// </summary>
        private async UniTaskVoid AsyncMoveLift(bool isDown, CancellationToken ct = default)
        {
            if (!_isOperatable) return;

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
            if (!_isOperatable) return;

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

                //もし刃が止まっている時にループを抜けたいなら以下の処理を入れる
                //好みだと思う
                if (!isCutting && _nowCutterSpeed == 0) break;
            }
        }

        private void SetCameraTransform()
        {
            if (PhotonNetwork.IsConnected && !photonView.IsMine) return;

            reaperCameraTransform.position = transform.TransformPoint(_cameraOffsetPos.Value);
            reaperCameraTransform.eulerAngles = transform.eulerAngles + _cameraOffsetRot.Value;
        }

        [PunRPC]
        public void RPCRotateCutter(bool isRotate)
        {
            if (!_isOperatable) return;

            _cutterCancellationTokenSource?.Cancel();
            _cutterCancellationTokenSource = new();
            AsyncRotateCutter(isRotate, _cutterCancellationTokenSource.Token).Forget();

            _isCutting.Value = isRotate;

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
            if (!_isOperatable) return;

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

