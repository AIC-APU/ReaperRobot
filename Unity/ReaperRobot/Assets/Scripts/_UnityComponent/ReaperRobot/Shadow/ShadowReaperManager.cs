using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using System.Threading;

namespace ReaperRobot.Scripts.UnityComponent.ReaperRobot
{
    public class ShadowReaperManager : MonoBehaviour
    {
        #region Serialized Private Fields
        [SerializeField] private Transform _reaper;
        [SerializeField] private Transform _cutterR;
        [SerializeField] private Transform _cutterL;

        //アニメーションも再現するなら以下を使う
        //[SerializeField] private Animator _crawlerL;
        //[SerializeField] private Animator _crawlerR;
        #endregion

        #region Private Fields
        public IReadOnlyReactiveProperty<bool> IsCutting => _isCutting;
        private ReactiveProperty<bool> _isCutting = new(true);
        public IReadOnlyReactiveProperty<bool> IsLiftDown => _isLiftDown;
        private ReactiveProperty<bool> _isLiftDown = new(true);

        private CancellationTokenSource _liftCancellationTokenSource = new();
        private CancellationTokenSource _cutterCancellationTokenSource = new();
        private float _nowCutterSpeed = 0f;
        #endregion

        #region MonoBehaviour Callbacks
        private void Start()
        {
            //isLiftDownの購読
            _isLiftDown.Subscribe(isDown =>
            {
                _liftCancellationTokenSource?.Cancel();
                _liftCancellationTokenSource = new();
                AsyncMoveLift(isDown, _liftCancellationTokenSource.Token).Forget();
            }).AddTo(this);

            //isCuttingの購読
            _isCutting.Subscribe(isRotate =>
            {
                _cutterCancellationTokenSource?.Cancel();
                _cutterCancellationTokenSource = new();
                AsyncRotateCutter(isRotate, _cutterCancellationTokenSource.Token).Forget();
            }).AddTo(this);
        }

        private void OnDestroy()
        {
            //非同期処理の停止            
            _liftCancellationTokenSource?.Cancel();
            _cutterCancellationTokenSource?.Cancel();
        }
        #endregion

        #region Public method
        public void MoveLift(bool isDown)
        {
            _isLiftDown.Value = isDown;
        }

        public void RotateCutter(bool isCutting)
        {
            _isCutting.Value = isCutting;
        }
        #endregion

        #region Private method
        /// <summary>
        /// isCuttingがtrueならリフトを下げる、falseなら上げる非同期処理
        /// </summary>
        private async UniTaskVoid AsyncMoveLift(bool isDown, CancellationToken ct = default)
        {
            var reaperTransform = _reaper;
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

                await UniTask.Yield(PlayerLoopTiming.Update, ct);

                //刃が止まったらループを抜ける
                if (!isCutting && _nowCutterSpeed == 0) break;
            }
        }
        #endregion
    }
}
