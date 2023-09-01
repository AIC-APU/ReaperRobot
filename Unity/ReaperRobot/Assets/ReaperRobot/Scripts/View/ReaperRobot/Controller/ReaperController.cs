using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;
using Cysharp.Threading.Tasks;

namespace Plusplus.ReaperRobot.Scripts.View.ReaperRobot
{
    [RequireComponent(typeof(PlayerInput))]
    public class ReaperController : MonoBehaviour
    {
        #region Serialized Private Fields    
        [SerializeField] private ReaperManager _reaperManager;
        [SerializeField] private bool _transmitterMode = true;
        [SerializeField] private float _delay = 0f;
        [SerializeField] private bool _enableDelay = false;
        #endregion

        #region private Fields
        private PlayerInput _playerInput;
        private InputActionMap _reaperActionMap;
        #endregion

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _reaperActionMap = _playerInput.actions.FindActionMap("Reaper");

            //操作対象がこのロボットでなくなったら止まる
            _reaperActionMap
                .ObserveEveryValueChanged(x => x.enabled)
                .Skip(1)
                .Where(x => !x)
                .Subscribe(_ => _reaperManager.Move(0, 0))
                .AddTo(this);
        }

        private void OnEnable()
        {
            _reaperActionMap["Brake"].started += Brake;
            _reaperActionMap["Brake"].canceled += OffBrake;
            _reaperActionMap["Lift"].started += MoveLift;
            _reaperActionMap["Cutter"].started += RotateCutter;
        }

        private void OnDisable()
        {
            _reaperActionMap["Brake"].started -= Brake;
            _reaperActionMap["Brake"].canceled -= OffBrake;
            _reaperActionMap["Lift"].started -= MoveLift;
            _reaperActionMap["Cutter"].started -= RotateCutter;
        }

        private void Update()
        {
            if (!_playerInput.enabled || !_reaperActionMap.enabled) return;

            var move = new Vector2();
            if (_transmitterMode)
            {
                move.x = _reaperActionMap["Turn"].ReadValue<float>();
                move.y = _reaperActionMap["FrontBack"].ReadValue<float>();
            }
            else
            {
                move = _reaperActionMap["Move"].ReadValue<Vector2>();
            }

            Move(move.x, move.y);
        }
        #endregion

        #region public method
        public void SwitchEnableDelay()
        {
            _enableDelay = !_enableDelay;
        }
        #endregion


        #region private method
        private async void Brake(InputAction.CallbackContext obj)
        {
            if (_enableDelay && _delay > 0) await UniTask.Delay((int)(_delay * 1000));
            _reaperManager.PutOnBrake();
        }
        private async void OffBrake(InputAction.CallbackContext obj)
        {
            if (_enableDelay && _delay > 0) await UniTask.Delay((int)(_delay * 1000));
            _reaperManager.ReleaseBrake();
        }
        private async void MoveLift(InputAction.CallbackContext obj)
        {
            if (_enableDelay && _delay > 0) await UniTask.Delay((int)(_delay * 1000));
            _reaperManager.MoveLift(!_reaperManager.IsLiftDown.Value);
        }
        private async void RotateCutter(InputAction.CallbackContext obj)
        {
            if (_enableDelay && _delay > 0) await UniTask.Delay((int)(_delay * 1000));
            _reaperManager.RotateCutter(!_reaperManager.IsCutting.Value);
        }
        private async void Move(float horizontal, float vertical)
        {
            if (_enableDelay && _delay > 0) await UniTask.Delay((int)(_delay * 1000));
            _reaperManager.Move(horizontal, vertical);
        }
        #endregion
    }

}
