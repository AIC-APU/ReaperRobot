using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace smart3tene.Reaper
{
    [RequireComponent(typeof(PlayerInput))]
    public class TestRobotController : MonoBehaviour
    {
        #region Public Fields
        public TestRobotManager _testRobotManager;
        #endregion

        #region Private Fields
        private PlayerInput _playerInput;
        private InputActionMap _actionMap;
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _actionMap = _playerInput.actions.FindActionMap("Reaper");
        }

        private void OnEnable()
        {
            _actionMap["Brake"].started += Brake;
            _actionMap["Brake"].canceled += OffBrake;
            _actionMap["ChangeMode"].started += StopMove;
            _actionMap["ChangeReaperAndPerson"].started += StopMove;
        }

        private void OnDisable()
        {
            _actionMap["Brake"].started -= Brake;
            _actionMap["Brake"].canceled -= OffBrake;
            _actionMap["ChangeMode"].started -= StopMove;
            _actionMap["ChangeReaperAndPerson"].started -= StopMove;
        }

        private void FixedUpdate()
        {
            if (!_playerInput.enabled || _playerInput.currentActionMap.name != "Reaper") return;

            var move = _actionMap["Move"].ReadValue<Vector2>();
            _testRobotManager.Move(move.x, move.y);
        }
        #endregion

        #region Private method
        private void Brake(InputAction.CallbackContext obj)
        {
            _testRobotManager.PutOnBrake();
        }
        private void OffBrake(InputAction.CallbackContext obj)
        {
            _testRobotManager.ReleaseBrake();
        }
        private void StopMove(InputAction.CallbackContext obj)
        {
            _testRobotManager.Move(0, 0);
        }
        #endregion
    }

}