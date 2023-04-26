using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

namespace Plusplus.ReaperRobot.Scripts.View.ReaperRobot
{
    [RequireComponent(typeof(PlayerInput))]
    public class ReaperController : MonoBehaviour
    {
        #region Serialized Private Fields    
        [SerializeField] private ReaperManager _reaperManager;
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

        private void FixedUpdate()
        {
            if (!_playerInput.enabled || !_reaperActionMap.enabled) return;

            var move = _reaperActionMap["Move"].ReadValue<Vector2>();
            _reaperManager.Move(move.x, move.y);
        }
        #endregion


        #region private method
        private void StopMove(InputAction.CallbackContext obj)
        {
            _reaperManager.Move(0, 0);
        }
        private void Brake(InputAction.CallbackContext obj)
        {
            _reaperManager.PutOnBrake();
        }
        private void OffBrake(InputAction.CallbackContext obj)
        {
            _reaperManager.ReleaseBrake();
        }
        private void MoveLift(InputAction.CallbackContext obj)
        {
            _reaperManager.MoveLift(!_reaperManager.IsLiftDown.Value);
        }
        private void RotateCutter(InputAction.CallbackContext obj)
        {
            _reaperManager.RotateCutter(!_reaperManager.IsCutting.Value);
        }
        #endregion
    }

}
