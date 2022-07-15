using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UniRx;

namespace smart3tene.Reaper
{
    [DefaultExecutionOrder(-1)]
    public class VRGUIManager : MonoBehaviour
    {
        #region Serialized private Fields
        [Header("Canvas Follow　Parameter")]
        [SerializeField] private bool _isImmediateMove;
        [SerializeField] private bool _isLockX;
        [SerializeField] private bool _isLockY;
        [SerializeField] private bool _isLockZ;

        [Header("ReapRate and Time")]
        [SerializeField] private TMP_Text _reaperRateNum;
        [SerializeField] private TMP_Text _timeNum;

        [Header("Mini Map")]
        [SerializeField] private Transform _miniMapCamera;

        [Header("Lift and Cutter")]
        [SerializeField] private Image _liftLamp;
        [SerializeField] private Image _cutterLamp;


        #endregion

        #region private Fields
        private Transform _mainCamera;
        private Transform _reaperTransform;
        private ReaperManager _reaperManager;
        #endregion

        #region Readonly Fields
        readonly float _positionOffset = 500f;
        readonly float _followMoveSpeed = 0.01f;
        readonly float _followRotateSpeed = 0.5f;
        readonly float _rotateSpeedThreshold = 0.9f;
        #endregion

        #region MonoBehaviour Callbacks

        void Awake()
        {
            _reaperManager = GameSystem.Instance.ReaperInstance.GetComponent<ReaperManager>();
            _reaperTransform = GameSystem.Instance.ReaperInstance.transform;

            //ReapRate
            GameSystem.Instance.CutGrassCount.Subscribe(x => _reaperRateNum.text = UpdateReapRate());

            //Time
            GameSystem.Instance.GameTime.Subscribe(x => _timeNum.text = UpdateGameTime(x));

            //Liftのランプ
            _reaperManager.IsLiftDown.Subscribe(isDown =>
            {
                if (isDown)
                {
                    _liftLamp.color = new Color32(255, 90, 0, 255);
                }
                else
                {
                    _liftLamp.color = new Color32(196, 196, 196, 255);
                }
            });


            //Cutterのランプ
            _reaperManager.IsCutting.Subscribe(isCutting =>
            {
                if (isCutting)
                {
                    _cutterLamp.color = new Color32(255, 90, 0, 255);
                }
                else
                {
                    _cutterLamp.color = new Color32(196, 196, 196, 255);
                }
            });

            _mainCamera = Camera.main.transform;
        }

        private void LateUpdate()
        {
            //ミニマップカメラの位置         
            _miniMapCamera.position = new Vector3(_reaperTransform.position.x, _miniMapCamera.position.y, _reaperTransform.position.z);
            _miniMapCamera.eulerAngles = new Vector3(_miniMapCamera.eulerAngles.x, _reaperTransform.eulerAngles.y, _miniMapCamera.eulerAngles.z);

            //Canvasをヌルっと追従させるときの処理
            if (_isImmediateMove) transform.position = _mainCamera.position;
            else transform.position = Vector3.Lerp(transform.position, _mainCamera.position + _mainCamera.forward * _positionOffset, _followMoveSpeed);

            var rotDif = _mainCamera.rotation * Quaternion.Inverse(transform.rotation);
            var rot = _mainCamera.rotation;
            if (_isLockX) rot.x = 0;
            if (_isLockY) rot.y = 0;
            if (_isLockZ) rot.z = 0;
            if (rotDif.w < _rotateSpeedThreshold) transform.rotation = Quaternion.Lerp(transform.rotation, rot, _followRotateSpeed * 4);
            else transform.rotation = Quaternion.Lerp(transform.rotation, rot, _followRotateSpeed);
        }
        #endregion

        #region public method
        //ボタンの挙動とか
        #endregion

        #region private method
        private string UpdateGameTime(float gameTime)
        {
            var span = new TimeSpan(0, 0, (int)gameTime);
            return span.ToString(@"hh\:mm\:ss");
        }

        private string UpdateReapRate()
        {
            float reapRate;
            if (GameSystem.Instance.AllGrassCount != 0)
            {
                reapRate = 100f * (float)GameSystem.Instance.CutGrassCount.Value / (float)GameSystem.Instance.AllGrassCount;
            }
            else
            {
                return "--";
            }

            return reapRate.ToString("F1");
        }
        #endregion
    }
}

