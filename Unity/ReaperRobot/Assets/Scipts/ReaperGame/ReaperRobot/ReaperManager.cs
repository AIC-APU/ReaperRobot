using Cysharp.Threading.Tasks;
using Photon.Pun;
using System.Threading;
using UniRx;
using UnityEngine;
using System;

namespace smart3tene.Reaper
{
    public class ReaperManager : MonoBehaviourPun, IPunObservable
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

        [Header("Center of Gravity")]
        [SerializeField] private Vector3 _centerOfGravity = new(0, 0, -0.2f);
        #endregion

        #region Public Field
        //入力された値を次の入力まで保持（記録のため）
        public Vector2 NowInput { get; private set; }

        //トルク関連の値
        public float rotateTorque = 100f;
        public float moveTorque = 300f;
        public float torqueRateAtCutting = 0.5f;
        #endregion

        #region Private
        //カッター&リフト関連
        public IReadOnlyReactiveProperty<bool> IsCutting => _isCutting;
        private ReactiveProperty<bool> _isCutting = new(true);
        public IReadOnlyReactiveProperty<bool> IsLiftDown => _isLiftDown;
        private ReactiveProperty<bool> _isLiftDown = new(true);

        private CancellationTokenSource _liftCancellationTokenSource = new();
        private CancellationTokenSource _cutterCancellationTokenSource = new();
        private float _nowCutterSpeed = 0f;

        //タイヤのアニメーション関連
        private ReactiveProperty<int> _leftRpm = new(0);
        private ReactiveProperty<int> _rightRpm = new(0);
        #endregion

        #region Readonly Field
        readonly float brakeTorque = 500f;
        #endregion


        #region MonoBehaviour Callbacks
        private void Start()
        {            
            //重心の設定
            GetComponent<Rigidbody>().centerOfMass = _centerOfGravity;

            //rpmの購読
            //crawlerアニメーションの処理
            //素のrpmは値が大きすぎるので、直進時の最大rpm = 70f（計測値）で除算している
            _leftRpm
                .Subscribe(x => _crawlerL.SetFloat("WheelTorque", (float)x / 70f))
                .AddTo(this);

            _rightRpm
                .Subscribe(x => _crawlerR.SetFloat("WheelTorque", (float)x / 70f))
                .AddTo(this);

            //isLiftDownの購読
            _isLiftDown.
                Subscribe(isDown =>
                {
                    _liftCancellationTokenSource?.Cancel();
                    _liftCancellationTokenSource = new();
                    AsyncMoveLift(isDown, _liftCancellationTokenSource.Token).Forget();
                })
                .AddTo(this);

            //isCuttingの購読
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
            _leftRpm.Value = (int)_wheelColliderL2.rpm;
            _rightRpm.Value = (int)_wheelColliderR2.rpm;       
        }

        private void OnDestroy()
        {
            //非同期処理の停止            
            _liftCancellationTokenSource?.Cancel();
            _cutterCancellationTokenSource?.Cancel();
        }
        #endregion


        #region public method
        /// <summary>
        /// Reaperロボットを移動させる関数。FixedUpdateのタイミングで呼ぶこと
        /// </summary>
        /// <param name="horizontal">水平方向の入力。-1~+1の範囲</param>
        /// <param name="vertical">垂直方向の入力。-1~+1の範囲</param>
        public void Move(float horizontal, float vertical, bool useRPC = true)
        {
            if(horizontal == 0 && vertical == 0 && _wheelColliderL2.motorTorque == 0 && _wheelColliderR2.motorTorque == 0)
            {
                return;
            }

            //入力値の範囲を制限
            horizontal = Mathf.Clamp(horizontal, -1, 1);
            vertical = Mathf.Clamp(vertical, -1, 1);

            NowInput = new Vector2(horizontal, vertical);

            //左右車輪のトルクを計算
            var torqueL = moveTorque * vertical;
            var torqueR = moveTorque * vertical;

            torqueL += rotateTorque * horizontal;
            torqueR -= rotateTorque * horizontal;

            if (_isCutting.Value)
            {
                torqueL *= torqueRateAtCutting;
                torqueR *= torqueRateAtCutting;
            }

            _wheelColliderL2.motorTorque = torqueL;
            _wheelColliderL3.motorTorque = torqueL;
            _wheelColliderR2.motorTorque = torqueR;
            _wheelColliderR3.motorTorque =  torqueR;

            //モーター音



            //入力値を同期させる
            if (useRPC && PhotonNetwork.IsConnected)
            {
                //float値をint値に変換して通信する
                //入力時のfloat値が小数点以下7桁まである数なので、千万で割る
                int horizontalInt = (int)(horizontal * 10000000f);
                int verticalInt = (int)(vertical * 10000000f);

                photonView.RPC(nameof(RPCMove), RpcTarget.Others, horizontalInt, verticalInt);
            }
        }

        public void PutOnBrake()
        {
            _wheelColliderL2.brakeTorque = brakeTorque;
            _wheelColliderL3.brakeTorque = brakeTorque;
            _wheelColliderR2.brakeTorque = brakeTorque;
            _wheelColliderR3.brakeTorque = brakeTorque;
        }

        public void ReleaseBrake()
        {
            _wheelColliderL2.brakeTorque = 0;
            _wheelColliderL3.brakeTorque = 0;
            _wheelColliderR2.brakeTorque = 0;
            _wheelColliderR3.brakeTorque = 0;
        }

        public void MoveLift(bool isDown)
        {
            _isLiftDown.Value = isDown;

            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC(nameof(RPCMoveLift), RpcTarget.Others, isDown);
            }
        }
        
        public void RotateCutter(bool isRotate)
        {
            _isCutting.Value = isRotate;

            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC(nameof(RPCRotateCutter), RpcTarget.Others, isRotate);
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
        #endregion

        #region RPC Methods
        [PunRPC]
        private void RPCMove(int horizontalInt, int verticalInt)
        {
            float horizontal = (float)horizontalInt / 10000000f;
            float vertical = (float)verticalInt / 10000000f;

            Move(horizontal, vertical, false);
        }

        [PunRPC]
        private void RPCMoveLift(bool isLiftDown)
        {
            _isLiftDown.Value = isLiftDown;
        }

        [PunRPC]
        private void RPCRotateCutter(bool isCutting)
        {
            _isCutting.Value = isCutting;
        }

        [PunRPC]
        private void RPCSyncTransform(Vector3 pos, Quaternion rot)
        {
            transform.position = pos;
            transform.rotation = rot;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(transform.position);
                stream.SendNext(transform.eulerAngles);
                Debug.Log("Send");
            }
            else
            {
                transform.position = (Vector3)stream.ReceiveNext();
                transform.eulerAngles = (Vector3)stream.ReceiveNext();
                Debug.Log("Receive");
            }
        }
        #endregion
    }
}

