using UnityEngine;
using UnityEngine.InputSystem;

namespace smart3tene.Reaper
{
    [RequireComponent(typeof(PlayerInput))]
    public class ReaperController : MonoBehaviour, IRobotController
    {
        #region Public Fields    
        public GameObject TargetRobot { get => _targetRobot; set => _targetRobot = value; }
        [SerializeField] private GameObject _targetRobot = null;
        #endregion

        #region private Fields
        private ReaperManager _reaperManager;
        private PlayerInput _playerInput;
        private InputActionMap _reaperActionMap;
        #endregion

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            if(TargetRobot == null)
            {
                TargetRobot = InstanceHolder.Instance.ReaperInstance;
            }

            _reaperManager      = _targetRobot.GetComponent<ReaperManager>();
            _playerInput        = GetComponent<PlayerInput>();


            _reaperActionMap       = _playerInput.actions.FindActionMap("Reaper");

            _reaperActionMap["Brake"].started                  += Brake;
            _reaperActionMap["Brake"].canceled                 += OffBrake;
            _reaperActionMap["Lift"].started                   += MoveLift;
            _reaperActionMap["Cutter"].started                 += RotateCutter;

            _reaperActionMap["ChangeMode"].started             += StopMove;
            _reaperActionMap["ChangeReaperAndPerson"].started  += StopMove;
        }

        private void OnDisable()
        {
            _reaperActionMap["Brake"].started                  -= Brake;
            _reaperActionMap["Brake"].canceled                 -= OffBrake;
            _reaperActionMap["Lift"].started                   -= MoveLift;
            _reaperActionMap["Cutter"].started                 -= RotateCutter;
            
            _reaperActionMap["ChangeMode"].started             -= StopMove;
            _reaperActionMap["ChangeReaperAndPerson"].started  -= StopMove;
        }

        private void FixedUpdate()
        {
            if (_playerInput.currentActionMap.name != "Reaper") return;

            var move = _reaperActionMap["Move"].ReadValue<Vector2>();
            _reaperManager.AsyncMove(move.x, move.y);
        }
        #endregion


        #region private method
        private void StopMove(InputAction.CallbackContext obj)
        {
            _reaperManager.AsyncMove(0,0);
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
