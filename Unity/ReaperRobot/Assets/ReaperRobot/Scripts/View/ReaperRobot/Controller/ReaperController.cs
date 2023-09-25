using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

namespace Plusplus.ReaperRobot.Scripts.View.ReaperRobot
{
    [RequireComponent(typeof(PlayerInput))]
    public class ReaperController : MonoBehaviour
    {
        #region Serialized Private Fields
        [Header("Reaper Manager")]
        [SerializeField] private ReaperManager _reaperManager;

        [Header("Transmitter Mode")]
        [SerializeField] private bool _transmitterMode = true;

        [Header("Delay")]
        [SerializeField] private bool _enableDelay = false;
        [SerializeField] private float _delay = 0f;
        #endregion

        #region Unity Event
        [Header("Unity Event")]
        public UnityEvent OnFirstInput;
        #endregion


        #region private Fields
        private PlayerInput _playerInput;
        private InputActionMap _reaperActionMap;
        private ReactiveProperty<bool> _firstMoveFlag = new(false);
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

            _firstMoveFlag
                .Where(x => x)
                .Subscribe(_ => OnFirstInput.Invoke())
                .AddTo(this);
        }

        private void OnEnable()
        {
            _reaperActionMap["Brake"].started += Brake;
            _reaperActionMap["Brake"].canceled += OffBrake;
            _reaperActionMap["CutterOn"].started += RotateCutter;
            _reaperActionMap["CutterOff"].started += StopCutter;
        }

        private void OnDisable()
        {
            _reaperActionMap["Brake"].started -= Brake;
            _reaperActionMap["Brake"].canceled -= OffBrake;
            _reaperActionMap["CutterOn"].started -= RotateCutter;
            _reaperActionMap["CutterOff"].started -= StopCutter;
        }

        private void Update()
        {
            if (!_playerInput.enabled || !_reaperActionMap.enabled) return;

            //Moveの処理
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

            //Liftの処理
            if (_reaperActionMap["LiftDown"].IsPressed() && !_reaperActionMap["LiftUp"].IsPressed())
            {
                MoveLift(true);
            }
            else if(!_reaperActionMap["LiftDown"].IsPressed() && _reaperActionMap["LiftUp"].IsPressed())
            {
                MoveLift(false);
            }
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
        
        private async void RotateCutter(InputAction.CallbackContext obj)
        {
            if (_enableDelay && _delay > 0) await UniTask.Delay((int)(_delay * 1000));
            _reaperManager.RotateCutter(true);
        }
        private async void StopCutter(InputAction.CallbackContext obj)
        {
            if (_enableDelay && _delay > 0) await UniTask.Delay((int)(_delay * 1000));
            _reaperManager.RotateCutter(false);
        }
        private async void Move(float horizontal, float vertical)
        {
            if (_enableDelay && _delay > 0) await UniTask.Delay((int)(_delay * 1000));
            if (!_firstMoveFlag.Value && (horizontal != 0 || vertical != 0)) _firstMoveFlag.Value = true;
            _reaperManager.Move(horizontal, vertical);
        }
        private async void MoveLift(bool IsLiftDown)
        {
            if (_enableDelay && _delay > 0) await UniTask.Delay((int)(_delay * 1000));
            _reaperManager.MoveLift(IsLiftDown);
        }
        #endregion
    }

}
