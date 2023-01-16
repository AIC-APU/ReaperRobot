using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

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
        #endregion

        #region private Fields
        private Transform _mainCamera;
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
            _mainCamera = Camera.main.transform;
        }

        private void LateUpdate()
        {
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
    }
}

