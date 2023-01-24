using UnityEngine;
using UnityEngine.InputSystem;

namespace smart3tene.Reaper
{
    [RequireComponent(typeof(PlayerInput))]
    public class ReaperController : MonoBehaviour
    {
        #region Serialized Private Fields    
        [SerializeField] private GameObject TargetRobot;
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

            _reaperManager      = TargetRobot.GetComponent<ReaperManager>();
            _playerInput        = GetComponent<PlayerInput>();

            _reaperActionMap       = _playerInput.actions.FindActionMap("Reaper");
        }

        private void OnEnable()
        {
            _reaperActionMap["Brake"].started += Brake;
            _reaperActionMap["Brake"].canceled += OffBrake;
            _reaperActionMap["Lift"].started += MoveLift;
            _reaperActionMap["Cutter"].started += RotateCutter;

            _reaperActionMap["ChangeMode"].started += StopMove;
            _reaperActionMap["ChangeReaperAndPerson"].started += StopMove;
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
            if (!_playerInput.enabled || _playerInput.currentActionMap.name != "Reaper") return;

            var move = _reaperActionMap["Move"].ReadValue<Vector2>();
            _reaperManager.Move(move.x, move.y);
        }
        #endregion


        #region private method
        private void StopMove(InputAction.CallbackContext obj)
        {
            _reaperManager.Move(0,0);
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
