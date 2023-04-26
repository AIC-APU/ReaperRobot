using UnityEngine;
using UnityEngine.InputSystem;

namespace Plusplus.ReaperRobot.Scripts.View.ReaperRobot
{
    [RequireComponent(typeof(PlayerInput))]
    public class ReaperControllerVR : MonoBehaviour
    {
        #region Serialized Private Fields    
        [SerializeField] private GameObject TargetRobot;
        #endregion

        #region Private Fields
        private ReaperManager _reaperManager;
        private PlayerInput _playerInput;
        private InputActionMap _reaperActionMap;
        #endregion

        #region MonoBehaviour Callbacks
        private void Awake()
        {
            _reaperManager = TargetRobot.GetComponent<ReaperManager>();
            _playerInput = GetComponent<PlayerInput>(); 
            _reaperActionMap = _playerInput.actions.FindActionMap("Reaper");

            _reaperActionMap["Move"].performed += Move;
            _reaperActionMap["Move"].canceled += Stop;
            _reaperActionMap["Brake"].started += Brake;
            _reaperActionMap["Brake"].canceled += OffBrake;
            _reaperActionMap["Lift"].started += MoveLift;
            _reaperActionMap["Cutter"].started += RotateCutter;
        }

        private void OnDisable()
        {
            _reaperActionMap["Move"].performed -= Move;
            _reaperActionMap["Brake"].started -= Brake;
            _reaperActionMap["Brake"].canceled -= Stop;
            _reaperActionMap["Brake"].canceled -= OffBrake;
            _reaperActionMap["Lift"].started -= MoveLift;
            _reaperActionMap["Cutter"].started -= RotateCutter;
        }

        #endregion

        #region Private method
        private void Move(InputAction.CallbackContext obj)
        {
            var move = _reaperActionMap["Move"].ReadValue<Vector2>();
            _reaperManager.Move(move.x, move.y);
        }
        private void Stop(InputAction.CallbackContext obj)
        {
            if (obj.control.name == "thumbstick") return;

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
