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

        [Header("Camera")]
        [SerializeField] private Transform _reaperCamera;
        #endregion


        #region private & readonly Field
        private bool _isOperatable = true;
        private bool _isCameraOperatable = true;

        //�J����
        public IReadOnlyReactiveProperty<Vector3> CameraOffsetPos => _cameraOffsetPos;
        private ReactiveProperty<Vector3> _cameraOffsetPos = new();

        public IReadOnlyReactiveProperty<Vector3> CameraOffsetRot => _cameraOffsetRot;
        private ReactiveProperty<Vector3> _cameraOffsetRot = new();

        readonly Vector3 cameraDefaultOffsetPos = new(0f, 1.2f, -0.5f);
        readonly Vector3 cameraDefaultOffsetRot = new(30f, 0f, 0f);


        //�J�b�^�[&���t�g�֘A
        public IReadOnlyReactiveProperty<bool> IsCutting => _isCutting;
        private ReactiveProperty<bool> _isCutting = new(true);
        public IReadOnlyReactiveProperty<bool> IsLiftDown => _isLiftDown;
        private ReactiveProperty<bool> _isLiftDown = new(true);

        private CancellationTokenSource _liftCancellationTokenSource = new();
        private CancellationTokenSource _cutterCancellationTokenSource = new();
        private float _nowCutterSpeed = 0f;


        //Wheel Collider�֘A
        readonly float rotateTorqueMultiplier = 100f;
        readonly float moveTorqueMultiplier = 300f;
        readonly float brakeTorque = 500f;

        public IReadOnlyReactiveProperty<float> LeftRpm => _leftRpm;
        private ReactiveProperty<float> _leftRpm = new(0);
        public IReadOnlyReactiveProperty<float> RightRpm => _rightRpm;
        private ReactiveProperty<float> _rightRpm = new(0);
        #endregion


        #region MonoBehaviour Callbacks
        private void Awake()
        {
            if (PhotonNetwork.IsConnected && !photonView.IsMine) return;

            ResetCameraPos();
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
            if (PhotonNetwork.IsConnected && !photonView.IsMine) return;

            //crawler�A�j���[�V�����̏���
            //�f��rpm�͒l���傫������̂ŁA���i���̍ő�rpm�i�v���l�j�ŏ��Z���Ă���
            _leftRpm.Value = _wheelColliderL2.rpm;
            _rightRpm.Value = _wheelColliderR2.rpm;
            _crawlerL.SetFloat("WheelTorque", _leftRpm.Value / 70);
            _crawlerR.SetFloat("WheelTorque", _rightRpm.Value / 70);           
        }

        private void LateUpdate()
        {
            if (PhotonNetwork.IsConnected && !photonView.IsMine) return;

            //�J�����ʒu
            SetCameraTransform();
        }

        private void OnDestroy()
        {
            //�񓯊������̒�~
            _liftCancellationTokenSource?.Cancel();
            _cutterCancellationTokenSource?.Cancel();
        }
        #endregion


        #region public method
        /// <summary>
        /// Reaper���{�b�g���ړ�������֐��BFixedUpdate�̃^�C�~���O�ŌĂԂ���
        /// </summary>
        /// <param name="horizontal">���������̓��́B-1~+1�͈̔�</param>
        /// <param name="vertical">���������̓��́B-1~+1�͈̔�</param>
        public async UniTaskVoid AsyncMove(float horizontal, float vertical)
        {
            if (!_isOperatable) return;
            if (PhotonNetwork.IsConnected && !photonView.IsMine) return;

            //���̏�����FixedUpdate�̃^�C�~���O�ōs��
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate);

            //���͒l�͈̔͂𐧌�
            horizontal = Mathf.Clamp(horizontal, -1, 1);
            vertical = Mathf.Clamp(vertical, -1, 1);

            //���E�ԗւ̃g���N���v�Z
            var torqueL = moveTorqueMultiplier * vertical;
            var torqueR = moveTorqueMultiplier * vertical;

            torqueL += rotateTorqueMultiplier * horizontal;
            torqueR -= rotateTorqueMultiplier * horizontal;

            _wheelColliderL2.motorTorque = torqueL;
            _wheelColliderL3.motorTorque = torqueL;
            _wheelColliderR2.motorTorque = torqueR;
            _wheelColliderR3.motorTorque = torqueR;

            //���[�^�[��

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
            if (!_isOperatable) return;
            if (PhotonNetwork.IsConnected && !photonView.IsMine) return;

            _liftCancellationTokenSource?.Cancel();
            _liftCancellationTokenSource = new();
            AsyncMoveLift(isDown, _liftCancellationTokenSource.Token).Forget();

            _isLiftDown.Value = isDown;
        }

        public void RotateCutter(bool isRotate)
        {
            if (!_isOperatable) return;
            if (PhotonNetwork.IsConnected && !photonView.IsMine) return;

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

        public void ResetCameraPos()
        {
            if (!_isOperatable || !_isCameraOperatable) return;
            if (PhotonNetwork.IsConnected && !photonView.IsMine) return;

            _cameraOffsetPos.Value = cameraDefaultOffsetPos;
            _cameraOffsetRot.Value = cameraDefaultOffsetRot;
        }

        public void MoveCamera(float x, float y, float z)
        {
            if (!_isOperatable || !_isCameraOperatable) return;
            if (PhotonNetwork.IsConnected && !photonView.IsMine) return;

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
            if (PhotonNetwork.IsConnected && !photonView.IsMine) return;

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
        /// isCutting��true�Ȃ烊�t�g��������Afalse�Ȃ�グ��񓯊�����
        /// </summary>
        private async UniTaskVoid AsyncMoveLift(bool isDown, CancellationToken ct = default)
        {
            if (!_isOperatable) return;
            if (PhotonNetwork.IsConnected && !photonView.IsMine) return;

            var reaperTransform = _reaper.transform;
            var liftSpeed = 10f;
            if (isDown)
            {
                //0�x�܂Ń��t�g�������邽�߂̃��[�v
                while (GetConvertedLocalAngleX(reaperTransform) > 0)
                {
                    reaperTransform.Rotate(liftSpeed * Time.deltaTime, 0, 0);
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }
            }
            else
            {
                //20�x�܂Ń��t�g���グ�邽�߂̃��[�v
                while (GetConvertedLocalAngleX(reaperTransform) < 20)
                {
                    reaperTransform.Rotate(-liftSpeed * Time.deltaTime, 0, 0);
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }
            }

            //reaper�̊p�x���A0�x���N�_�Ɂ}180�x�ɂȂ�悤�ɕϊ����郍�[�J�����]�b�g�i�^�オ+90�x�j
            static float GetConvertedLocalAngleX(Transform reaper)
            {
                var reaperAngleX = reaper.localEulerAngles.x;
                return reaperAngleX >= 180f ? 360f - reaperAngleX : -reaperAngleX;
            }
        }

        /// <summary>
        ///  isCutting��true�Ȃ�J�b�^�[����]������Afalse�Ȃ��]���~�߂�񓯊�����
        /// </summary>
        private async UniTaskVoid AsyncRotateCutter(bool isCutting, CancellationToken ct = default)
        {
            if (!_isOperatable) return;
            if (PhotonNetwork.IsConnected && !photonView.IsMine) return;

            var maxRotateSpeed = 1000f;
            var minRotateSpeed = 0f;
            var acceleration = 3f;
            while (true)
            {
                //�n�̑��x�������i��������ł�������j
                _nowCutterSpeed += isCutting ? acceleration : -acceleration;
                _nowCutterSpeed = Mathf.Clamp(_nowCutterSpeed, minRotateSpeed, maxRotateSpeed);

                //��]
                _cutterL.Rotate(0, _nowCutterSpeed * Time.deltaTime, 0);
                _cutterR.Rotate(0, -_nowCutterSpeed * Time.deltaTime, 0);

                //���[�^�[��


                await UniTask.Yield(PlayerLoopTiming.Update, ct);

                //�����n���~�܂��Ă��鎞�Ƀ��[�v�𔲂������Ȃ�ȉ��̏���������
                //�D�݂��Ǝv��
                if (!isCutting && _nowCutterSpeed == 0) break;
            }
        }

        private void SetCameraTransform()
        {
            _reaperCamera.position = transform.TransformPoint(_cameraOffsetPos.Value);
            _reaperCamera.eulerAngles = transform.eulerAngles + _cameraOffsetRot.Value;
        }
        #endregion
    }
}

